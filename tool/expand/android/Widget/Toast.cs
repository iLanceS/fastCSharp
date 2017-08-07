using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Widget.Toast À©Õ¹
    /// </summary>
    public static class toast
    {
        /// <summary>
        /// Show
        /// </summary>
        /// <param name="toast"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void show(this Toast toast)
        {
            toast.Show();
        }
        /// <summary>
        /// View
        /// </summary>
        /// <param name="toast"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static View getView(this Toast toast)
        {
            return toast.View;
        }
        /// <summary>
        /// View
        /// </summary>
        /// <param name="toast"></param>
        /// <param name="view"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setView(this Toast toast, View view)
        {
            toast.View = view;
        }
        /// <summary>
        /// SetText
        /// </summary>
        /// <param name="toast"></param>
        /// <param name="text"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setText(this Toast toast, string text)
        {
            toast.SetText(text);
        }
        /// <summary>
        /// Duration
        /// </summary>
        /// <param name="toast"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setDuration(this Toast toast, ToastLength value)
        {
            toast.Duration = value;
        }
        /// <summary>
        /// Duration
        /// </summary>
        /// <param name="toast"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setDuration(this Toast toast, int value)
        {
            toast.Duration = (ToastLength)value;
        }

        /// <summary>
        /// MakeText
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Toast makeText(Context context, string text, ToastLength duration)
        {
            return Toast.MakeText(context, text, duration);
        }
        /// <summary>
        /// MakeText
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Toast makeText(Context context, string text, int duration)
        {
            return Toast.MakeText(context, text, (ToastLength)duration);
        }
    }
}