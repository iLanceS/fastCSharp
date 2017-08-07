using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.PM;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Content.PM.PackageManager À©Õ¹
    /// </summary>
    public static class packageManager
    {
        /// <summary>
        /// GetActivityInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="component"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ActivityInfo getActivityInfo(this PackageManager manager, ComponentName component, PackageInfoFlags flags)
        {
            return manager.GetActivityInfo(component, flags);
        }
        /// <summary>
        /// GetApplicationInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="packageName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ApplicationInfo getApplicationInfo(this PackageManager manager, string packageName, PackageInfoFlags flags)
        {
            return manager.GetApplicationInfo(packageName, flags);
        }
        /// <summary>
        /// GetApplicationInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="packageName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ApplicationInfo getApplicationInfo(this PackageManager manager, string packageName, int flags)
        {
            return manager.GetApplicationInfo(packageName, (PackageInfoFlags)flags);
        }
        /// <summary>
        /// GetPackageInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="packageName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static PackageInfo getPackageInfo(this PackageManager manager, string packageName, PackageInfoFlags flags)
        {
            return manager.GetPackageInfo(packageName, flags);
        }
        /// <summary>
        /// GetPackageInfo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="packageName"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static PackageInfo getPackageInfo(this PackageManager manager, string packageName, int flags)
        {
            return manager.GetPackageInfo(packageName, (PackageInfoFlags)flags);
        }
        /// <summary>
        /// CheckPermission
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="permName"></param>
        /// <param name="pkgName"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Permission checkPermission(this PackageManager manager, string permName, string pkgName)
        {
            return manager.CheckPermission(permName, pkgName);
        }
        /// <summary>
        /// GetApplicationLabel
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getApplicationLabel(this PackageManager manager, ApplicationInfo info)
        {
            return manager.GetApplicationLabel(info);
        }
        /// <summary>
        /// QueryIntentActivities
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="intent"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IList<ResolveInfo> queryIntentActivities(this PackageManager manager, Intent intent, PackageInfoFlags flags)
        {
            return manager.QueryIntentActivities(intent, flags);
        }
    }
}