using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.IO.ObjectInputStream À©Õ¹
    /// </summary>
    public static class objectInputStream
    {
        /// <summary>
        /// ReadObject
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Object readObject(this Java.IO.ObjectInputStream stream)
        {
            return stream.ReadObject();
        }
    }
}