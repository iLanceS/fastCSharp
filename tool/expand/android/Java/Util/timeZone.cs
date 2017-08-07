using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.TimeZone À©Õ¹
    /// </summary>
    public static class timeZone
    {
        /// <summary>
        /// Default
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static TimeZone getDefault()
        {
            return TimeZone.Default;
        }
        /// <summary>
        /// ID
        /// </summary>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getID(this TimeZone timeZone)
        {
            return timeZone.ID;
        }
    }
}