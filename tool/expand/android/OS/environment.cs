using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.Environment À©Õ¹
    /// </summary>
    public static class environment
    {
        /// <summary>
        /// ExternalStorageState
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getExternalStorageState()
        {
            return Android.OS.Environment.ExternalStorageState;
        }
        /// <summary>
        /// ExternalStorageDirectory
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.IO.File getExternalStorageDirectory()
        {
            return Android.OS.Environment.ExternalStorageDirectory;
        }
    }
}