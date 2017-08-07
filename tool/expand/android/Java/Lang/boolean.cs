using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Boolean À©Õ¹
    /// </summary>
    public static class boolean
    {
        /// <summary>
        /// BooleanValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool booleanValue(this Java.Lang.Boolean value)
        {
            return value.BooleanValue();
        }
    }
}