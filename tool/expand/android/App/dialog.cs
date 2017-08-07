using System;
using Android.App;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.App.Dialog À©Õ¹
    /// </summary>
    public static class dialog
    {
        /// <summary>
        /// Dismiss
        /// </summary>
        /// <param name="dialog"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void dismiss(this Dialog dialog)
        {
            dialog.Dismiss();
        }
        /// <summary>
        /// IsShowing
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isShowing(this Dialog dialog)
        {
            return dialog.IsShowing;
        }
        /// <summary>
        /// Show
        /// </summary>
        /// <param name="dialog"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void show(this Dialog dialog)
        {
            dialog.Show();
        }
    }
}