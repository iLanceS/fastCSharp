using System;
using Android.Graphics;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Graphics.Bitmap À©Õ¹
    /// </summary>
    public static class bitmap
    {
        /// <summary>
        /// Compress
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="format"></param>
        /// <param name="quality"></param>
        /// <param name="stream"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void compress(this Bitmap bitmap, Bitmap.CompressFormat format, int quality, System.IO.Stream stream)
        {
            bitmap.Compress(format, quality, stream);
        }
        /// <summary>
        /// Width
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getWidth(this Bitmap bitmap)
        {
            return bitmap.Width;
        }
        /// <summary>
        /// Height
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getHeight(this Bitmap bitmap)
        {
            return bitmap.Height;
        }
        /// <summary>
        /// Recycle
        /// </summary>
        /// <param name="bitmap"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void recycle(this Bitmap bitmap)
        {
            bitmap.Recycle();
        }
    }
}