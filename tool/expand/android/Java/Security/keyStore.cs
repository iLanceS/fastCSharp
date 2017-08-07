using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Security.KeyStore À©Õ¹
    /// </summary>
    public static class keyStore
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="keystore"></param>
        /// <param name="stream"></param>
        /// <param name="password"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void load(this Java.Security.KeyStore keystore, System.IO.Stream stream, char[] password)
        {
            keystore.Load(stream, password);
        }
    }
}