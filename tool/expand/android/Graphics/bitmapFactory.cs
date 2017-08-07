using System;
using Android.Graphics;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Graphics.BitmapFactory À©Õ¹
    /// </summary>
    public static class bitmapFactory
    {
        /// <summary>
        /// DecodeStream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Bitmap decodeStream(System.IO.Stream stream)
        {
           return BitmapFactory.DecodeStream(stream);
        }
        /// <summary>
        /// DecodeFile
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Bitmap decodeFile(string pathName, BitmapFactory.Options opts)
        {
            return BitmapFactory.DecodeFile(pathName, opts);
        }
    }
}