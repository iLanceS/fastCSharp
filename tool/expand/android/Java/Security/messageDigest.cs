using System;
using Java.Security;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Security.MessageDigest À©Õ¹
    /// </summary>
    public static class messageDigest
    {
        /// <summary>
        /// GetInstance
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static MessageDigest getInstance(string algorithm)
        {
            return MessageDigest.GetInstance(algorithm);
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="messageDigest"></param>
        /// <param name="input"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void update(this MessageDigest messageDigest,byte[] input)
        {
            messageDigest.Update(input);
        }
        /// <summary>
        /// Digest
        /// </summary>
        /// <param name="messageDigest"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte[] digest(this MessageDigest messageDigest)
        {
            return messageDigest.Digest();
        }
    }
}