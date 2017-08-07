using System;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.Message À©Õ¹
    /// </summary>
    public static class message
    {
        /// <summary>
        /// Obtain
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Message obtain()
        {
            return Android.OS.Message.Obtain();
        }
        /// <summary>
        /// SendToTarget
        /// </summary>
        /// <param name="message"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void sendToTarget(this Message message)
        {
            message.SendToTarget();
        }
    }
}