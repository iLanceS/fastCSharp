using System;
using Android.Content;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Content.Intent À©Õ¹
    /// </summary>
    public static class intent
    {
        /// <summary>
        /// AddFlags
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent addFlags(this Intent intent, ActivityFlags flags)
        {
            return intent.AddFlags(flags);
        }
        /// <summary>
        /// AddFlags
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent addFlags(this Intent intent, int flags)
        {
            return intent.AddFlags((ActivityFlags)flags);
        }
        /// <summary>
        /// SetData
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent setData(this Intent intent, Android.Net.Uri data)
        {
            return intent.SetData(data);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, string value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, double value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, double[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, IParcelable value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, IParcelable[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, bool value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, char value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, char[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, byte[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, Bundle value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, sbyte value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, bool[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, int[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, long value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, long[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, short value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, short[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, Java.Lang.ICharSequence value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, Java.Lang.ICharSequence[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, Java.IO.ISerializable value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, string[] value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, int value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, float value)
        {
            return intent.PutExtra(name, value);
        }
        /// <summary>
        /// PutExtra
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Intent putExtra(this Intent intent, string name, float[] value)
        {
            return intent.PutExtra(name, value);
        }
    }
}