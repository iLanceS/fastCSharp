using System;
using Java.Util;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Properties À©Õ¹
    /// </summary>
    public static class properties
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static mapEntry[] entrySet(this Properties properties)
        {
            return properties.StringPropertyNames().getArray(name => new mapEntry { Key = name, Properties = properties });
        }
    }
}