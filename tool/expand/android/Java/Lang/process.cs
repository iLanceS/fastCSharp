using Java.Lang;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Process À©Õ¹
    /// </summary>
    public static class process
    {
        /// <summary>
        /// InputStream
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IO.Stream getInputStream(this Process process)
        {
            return process.InputStream;
        }
    }
}