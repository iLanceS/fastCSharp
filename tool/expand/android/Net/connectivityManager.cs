using System;
using Android.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Net.ConnectivityManager À©Õ¹
    /// </summary>
    public static class connectivityManager
    {
        /// <summary>
        /// ActiveNetworkInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static NetworkInfo getActiveNetworkInfo(this ConnectivityManager manager)
        {
            return manager.ActiveNetworkInfo;
        }
        /// <summary>
        /// GetAllNetworkInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static NetworkInfo[] getAllNetworkInfo(this ConnectivityManager manager)
        {
            return manager.GetAllNetworkInfo();
        }
    }
}