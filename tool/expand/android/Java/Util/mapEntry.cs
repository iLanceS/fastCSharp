using System;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.MapEntry Ìæ´ú
    /// </summary>
    public struct mapEntry
    {
        /// <summary>
        /// 
        /// </summary>
        internal string Key;
        /// <summary>
        /// 
        /// </summary>
        internal Properties Properties;
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public string getKey()
        {
            return Key;
        }
        /// <summary>
        /// GetProperty
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public string getValue()
        {
            return Properties.GetProperty(Key);
        }
    }
}