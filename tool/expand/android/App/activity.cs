using System;
using Android.App;
using Android.Content;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.App.Activity À©Õ¹
    /// </summary>
    public static class activity
    {
        /// <summary>
        /// StartActivityForResult
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="intent"></param>
        /// <param name="requestCode"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void startActivityForResult(this Activity activity, Intent intent, int requestCode)
        {
            activity.StartActivityForResult(intent, requestCode);
        }
    }
}