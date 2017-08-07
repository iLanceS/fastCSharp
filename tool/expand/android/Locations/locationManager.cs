using System;
using Android.Locations;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Locations.LocationManager À©Õ¹
    /// </summary>
    public static class locationManager
    {
        /// <summary>
        /// GetBestProvider
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="criteria"></param>
        /// <param name="enabledOnly"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getBestProvider(this LocationManager manager, Criteria criteria, bool enabledOnly)
        {
            return manager.GetBestProvider(criteria, enabledOnly);
        }
        /// <summary>
        /// GetLastKnownLocation
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Location getLastKnownLocation(this LocationManager manager, string provider)
        {
            return manager.GetLastKnownLocation(provider);
        }
    }
}