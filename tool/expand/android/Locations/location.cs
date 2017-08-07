using System;
using Android.Locations;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Locations.Location À©Õ¹
    /// </summary>
    public static class location
    {
        /// <summary>
        /// Latitude
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double getLatitude(this Location location)
        {
            return location.Latitude;
        }
        /// <summary>
        /// Longitude
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double getLongitude(this Location location)
        {
            return location.Longitude;
        }
    }
}