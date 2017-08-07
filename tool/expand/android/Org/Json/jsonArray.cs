using System;
using Org.Json;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Json.JSONArray À©Õ¹
    /// </summary>
    public static class jsonArray
    {
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONArray put(this JSONArray jsonArray, int value)
        {
            return jsonArray.Put(value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONArray put(this JSONArray jsonArray, long value)
        {
            return jsonArray.Put(value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONArray put(this JSONArray jsonArray, double value)
        {
            return jsonArray.Put(value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONArray put(this JSONArray jsonArray, bool value)
        {
            return jsonArray.Put(value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONArray put(this JSONArray jsonArray, Java.Lang.Object value)
        {
            return jsonArray.Put(value);
        }
        /// <summary>
        /// Length
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int length(this JSONArray jsonArray)
        {
            return jsonArray.Length();
        }
    }
}