using System;
using Android.Content;

namespace fastCSharp.android
{
    public static class contentValues
    {
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, float value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, long value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, int value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, short value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, byte[] value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, string value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, double value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, bool value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void put(this ContentValues values, string key, sbyte value)
        {
            values.Put(key, value);
        }
        /// <summary>
        /// Clear
        /// </summary>
        /// <param name="values"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void clear(this ContentValues values)
        {
            values.Clear();
        }
    }
}