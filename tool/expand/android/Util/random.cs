using System;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Util.Random À©Õ¹
    /// </summary>
    public static class random
    {
        /// <summary>
        /// NextInt
        /// </summary>
        /// <param name="random"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int nextInt(this Java.Util.Random random, int n)
        {
            return random.NextInt(n);
        }
    }
}