using System;
using Android.Views;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Webkit.View À©Õ¹
    /// </summary>
    public static class view
    {
        /// <summary>
        /// LayoutParameters
        /// </summary>
        /// <param name="view"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setLayoutParams(this View view, ViewGroup.LayoutParams value)
        {
            view.LayoutParameters = value;
        }
    }
}