using System;
using Android.Views;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Views.IWindowManager À©Õ¹
    /// </summary>
    public static class iWindowManager
    {
        /// <summary>
        /// DefaultDisplay
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Display getDefaultDisplay(this IWindowManager manager)
        {
            return manager.DefaultDisplay;
        }
    }
}