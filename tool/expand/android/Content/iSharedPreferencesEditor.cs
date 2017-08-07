using System;
using Android.Content;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Content.ISharedPreferencesEditor À©Õ¹
    /// </summary>
    public static class iSharedPreferencesEditor
    {
        /// <summary>
        /// PutInt
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ISharedPreferencesEditor putInt(this ISharedPreferencesEditor editor, string key, int value)
        {
            return editor.PutInt(key, value);
        }
        /// <summary>
        /// PutLong
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ISharedPreferencesEditor putLong(this ISharedPreferencesEditor editor, string key, long value)
        {
            return editor.PutLong(key, value);
        }
        /// <summary>
        /// PutString
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ISharedPreferencesEditor putString(this ISharedPreferencesEditor editor, string key, string value)
        {
            return editor.PutString(key, value);
        }
        /// <summary>
        /// Commit
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool commit(this ISharedPreferencesEditor editor)
        {
            return editor.Commit();
        }
    }
}