using System;
using Android.Content;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.Content.ISharedPreferences À©Õ¹
    /// </summary>
    public static class iSharedPreferences
    {
        /// <summary>
        /// GetString
        /// </summary>
        /// <param name="sharedPreferences"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getString(this ISharedPreferences sharedPreferences, string key, string defValue)
        {
            return sharedPreferences.GetString(key, defValue);
        }
        /// <summary>
        /// GetInt
        /// </summary>
        /// <param name="sharedPreferences"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getInt(this ISharedPreferences sharedPreferences, string key, int defValue)
        {
            return sharedPreferences.GetInt(key, defValue);
        }
        /// <summary>
        /// GeLong
        /// </summary>
        /// <param name="sharedPreferences"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getLong(this ISharedPreferences sharedPreferences, string key, long defValue)
        {
            return sharedPreferences.GetLong(key, defValue);
        }
        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="sharedPreferences"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ISharedPreferencesEditor edit(this ISharedPreferences sharedPreferences)
        {
            return sharedPreferences.Edit();
        }
    }
}