using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Object À©Õ¹
    /// </summary>
    public static class objectExpand
    {
        /// <summary>
        /// Class
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Class getClass(this Java.Lang.Object value)
        {
            return value.Class;
        }
    }
}