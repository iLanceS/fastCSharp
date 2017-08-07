using System;
using System.Collections.Generic;
using Android.App;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.App.ActivityManager À©Õ¹
    /// </summary>
    public static class activityManager
    {
        /// <summary>
        /// RunningAppProcesses
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IList<ActivityManager.RunningAppProcessInfo> getRunningAppProcesses(this ActivityManager manager)
        {
            return manager.RunningAppProcesses;
        }
        /// <summary>
        /// GetMemoryInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="memoryinfo"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void getMemoryInfo(this ActivityManager manager, ActivityManager.MemoryInfo memoryinfo)
        {
            manager.GetMemoryInfo(memoryinfo);
        }
    }
}