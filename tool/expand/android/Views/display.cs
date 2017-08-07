using System;
using Android.Util;
using Android.Views;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Views.Display À©Õ¹
    /// </summary>
    public static class display
    {
        /// <summary>
        /// GetMetrics
        /// </summary>
        /// <param name="display"></param>
        /// <param name="outMetrics"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void getMetrics(this Display display, DisplayMetrics outMetrics)
        {
            display.GetMetrics(outMetrics);
        }
        /// <summary>
        /// Width
        /// </summary>
        /// <param name="display"></param>
        /// <returns></returns>
        [Obsolete("deprecated")]
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getWidth(this Display display)
        {
            return display.Width;
        }
        /// <summary>
        /// Height
        /// </summary>
        /// <param name="display"></param>
        /// <returns></returns>
        [Obsolete("deprecated")]
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getHeight(this Display display)
        {
            return display.Height;
        }
    }
}