using System;
using Android.Telephony;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Telephony.TelephonyManager À©Õ¹
    /// </summary>
    public static class telephonyManager
    {
        /// <summary>
        /// DeviceId
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getDeviceId(this TelephonyManager manager)
        {
            return manager.DeviceId;
        }
        /// <summary>
        /// SimOperator
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getSimOperator(this TelephonyManager manager)
        {
            return manager.SimOperator;
        }
        /// <summary>
        /// NetworkType
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static NetworkType getNetworkType(this TelephonyManager manager)
        {
            return manager.NetworkType;
        }
        /// <summary>
        /// SimSerialNumber
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getSimSerialNumber(this TelephonyManager manager)
        {
            return manager.SimSerialNumber;
        }
    }
}