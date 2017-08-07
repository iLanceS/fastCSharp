using System;
using Android.Graphics;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Graphics.Matrix À©Õ¹
    /// </summary>
    public static  class matrix
    {
        /// <summary>
        /// PostScale
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void postScale(this Matrix matrix, float sx, float sy)
        {
            matrix.PostScale(sx, sy);
        }
    }
}