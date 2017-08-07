using System;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.Handler À©Õ¹
    /// </summary>
    public static class handler
    {
        /// <summary>
        /// HasMessages
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="what"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool hasMessages(this Handler handler, int what)
        {
            return handler.HasMessages(what);
        }
        /// <summary>
        /// RemoveMessages
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="what"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void removeMessages(this Handler handler, int what)
        {
            handler.RemoveMessages(what);
        }
        /// <summary>
        /// SendEmptyMessage
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="what"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sendEmptyMessage(this Handler handler, int what)
        {
            return handler.SendEmptyMessage(what);
        }
        /// <summary>
        /// Post
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="runnable"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool post(this Handler handler, Java.Lang.IRunnable runnable)
        {
            return handler.Post(runnable);
        }
        /// <summary>
        /// Looper
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Looper getLooper(this Handler handler)
        {
            return handler.Looper;
        }
        /// <summary>
        /// SendMessageDelayed
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="msg"></param>
        /// <param name="delayMillis"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sendMessageDelayed(this Handler handler, Message msg, long delayMillis)
        {
            return handler.SendMessageDelayed(msg, delayMillis);
        }
        /// <summary>
        /// ObtainMessage
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="what"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Message obtainMessage(this Handler handler, int what, Java.Lang.Object obj)
        {
            return handler.ObtainMessage(what, obj);
        }
        /// <summary>
        /// ObtainMessage
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Message obtainMessage(this Handler handler)
        {
            return handler.ObtainMessage();
        }
        /// <summary>
        /// ObtainMessage
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="what"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Message obtainMessage(this Handler handler, int what)
        {
            return handler.ObtainMessage(what);
        }
        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sendMessage(this Handler handler, Message message)
        {
            return handler.SendMessage(message);
        }
    }
}