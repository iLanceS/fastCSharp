using Android.Net;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Net.Uri À©Õ¹
    /// </summary>
    public static class uri
    {
        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string decode(string url)
        {
            return Uri.Decode(url);
        }
        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Uri parse(string url)
        {
            return Uri.Parse(url);
        }
        /// <summary>
        /// Scheme
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getScheme(this Uri uri)
        {
            return uri.Scheme;
        }
    }
}