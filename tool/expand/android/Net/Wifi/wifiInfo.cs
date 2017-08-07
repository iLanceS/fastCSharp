using System;
using Android.Net.Wifi;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Net.Wifi.WifiInfo À©Õ¹
    /// </summary>
    public static class wifiInfo
    {
        /// <summary>
        /// MacAddress
        /// </summary>
        /// <param name="wifi"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getMacAddress(this WifiInfo wifi)
        {
            return wifi.MacAddress;
        }
        /// <summary>
        /// BSSID
        /// </summary>
        /// <param name="wifi"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getBSSID(this WifiInfo wifi)
        {
            return wifi.BSSID;
        }
        /// <summary>
        /// SSID
        /// </summary>
        /// <param name="wifi"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getSSID(this WifiInfo wifi)
        {
            return wifi.SSID;
        }
    }
}