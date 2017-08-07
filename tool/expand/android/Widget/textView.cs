using System;
using Android.Widget;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Widget.TextView À©Õ¹
    /// </summary>
    public static class textView
    {
        /// <summary>
        /// Text
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="text"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setText(this TextView textView, string text)
        {
            textView.Text = text;
        }
    }
}