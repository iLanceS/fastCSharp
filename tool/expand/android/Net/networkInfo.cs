using System;
using Android.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Net.NetworkInfo À©Õ¹
    /// </summary>
    public static class networkInfo
    {
        /// <summary>
        /// IsAvailable
        /// </summary>
        /// <param name="networkInfo"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isAvailable(this NetworkInfo networkInfo)
        {
            return networkInfo.IsAvailable;
        }
        /// <summary>
        /// Type
        /// </summary>
        /// <param name="networkInfo"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ConnectivityType getType(this NetworkInfo networkInfo)
        {
            return networkInfo.Type;
        }
        /// <summary>
        /// TypeName
        /// </summary>
        /// <param name="networkInfo"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getTypeName(this NetworkInfo networkInfo)
        {
            return networkInfo.TypeName;
        }
        /// <summary>
        /// IsConnected
        /// </summary>
        /// <param name="networkInfo"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isConnected(this NetworkInfo networkInfo)
        {
            return networkInfo.IsConnected;
        }
        /// <summary>
        /// ExtraInfo
        /// </summary>
        /// <param name="networkInfo"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getExtraInfo(this NetworkInfo networkInfo)
        {
            return networkInfo.ExtraInfo;
        }
    }
}