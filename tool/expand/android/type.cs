using System;

namespace fastCSharp.android
{
    /// <summary>
    /// ¿‡–Õ¿©’π
    /// </summary>
    public static class type
    {
        /// <summary>
        /// Name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getName(this Type type)
        {
            return type.Name;
        }
    }
}