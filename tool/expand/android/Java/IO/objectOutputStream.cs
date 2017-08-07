using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.ObjectOutputStream À©Õ¹
    /// </summary>
    public static class objectOutputStream
    {
        /// <summary>
        /// WriteObject
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void writeObject(this Java.IO.ObjectOutputStream stream, Java.Lang.Object value)
        {
            stream.WriteObject(value);
        }
    }
}