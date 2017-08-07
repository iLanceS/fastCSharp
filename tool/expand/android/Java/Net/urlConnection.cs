using System;
using Java.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Net.URLConnection À©Õ¹
    /// </summary>
    public static class urlConnection
    {
        /// <summary>
        /// DoInput
        /// </summary>
        /// <param name="url"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setDoInput(this URLConnection url, bool value)
        {
            url.DoInput = value;
        }
        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="url"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void connect(this URLConnection url)
        {
            url.Connect();
        }
        /// <summary>
        /// InputStream
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream getInputStream(this URLConnection url)
        {
           return url.InputStream;
        }
    }
}