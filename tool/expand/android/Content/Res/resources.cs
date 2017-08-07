using System;
using Android.Content.Res;
using Android.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Content.Res.Resources À©Õ¹
    /// </summary>
    public static class resources
    {
        /// <summary>
        /// DisplayMetrics
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static DisplayMetrics getDisplayMetrics(this Resources resources)
        {
            return resources.DisplayMetrics;
        }
    }
}