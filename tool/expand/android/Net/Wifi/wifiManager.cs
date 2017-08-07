using System;
using Android.Net.Wifi;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Net.Wifi.WifiManager À©Õ¹
    /// </summary>
    public static class wifiManager
    {
        /// <summary>
        /// ConnectionInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static WifiInfo getConnectionInfo(this WifiManager manager)
        {
            return manager.ConnectionInfo;
        }
    }
}