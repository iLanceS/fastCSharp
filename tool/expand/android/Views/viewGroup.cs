using System;
using Android.Views;

namespace fastCSharp.android.Views
{
    /// <summary>
    /// Android.Views.ViewGroup À©Õ¹
    /// </summary>
    public static class viewGroup
    {
        /// <summary>
        /// AddView
        /// </summary>
        /// <param name="view"></param>
        /// <param name="child"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void addView(this ViewGroup view, View child)
        {
            view.AddView(child);
        }
    }
}