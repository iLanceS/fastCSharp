using System;
using Java.Util;
using Org.Json;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Json.JSONObject 扩展
    /// </summary>
    public static class jsonObject
    {
        /// <summary>
        /// 根据名称获取整数 OptInt
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int optInt(this JSONObject json, string name)
        {
            return json.OptInt(name);
        }
        /// <summary>
        /// OptInt
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int optInt(this JSONObject json, string name, int fallback)
        {
            return json.OptInt(name, fallback);
        }
        /// <summary>
        /// 根据名称获取数据 Opt
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Object opt(this JSONObject json, string name)
        {
            return json.Opt(name);
        }
        /// <summary>
        /// 判断是否存在名称 IsNull
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isNull(this JSONObject json, string name)
        {
            return json.IsNull(name);
        }
        /// <summary>
        /// 根据名称获取字符串 GetString
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getString(this JSONObject json, string name)
        {
            return json.GetString(name);
        }
        /// <summary>
        /// 根据名称获取整数 GetLong
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long getLong(this JSONObject json, string name)
        {
            return json.GetLong(name);
        }
        /// <summary>
        /// 根据名称获取整数 GetInt
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getInt(this JSONObject json, string name)
        {
            return json.GetInt(name);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONObject put(this JSONObject json, string name, long value)
        {
            return json.Put(name, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONObject put(this JSONObject json, string name, int value)
        {
            return json.Put(name, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONObject put(this JSONObject json, string name, double value)
        {
            return json.Put(name, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONObject put(this JSONObject json, string name, bool value)
        {
            return json.Put(name, value);
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONObject put(this JSONObject json, string name, Java.Lang.Object value)
        {
            return json.Put(name, value);
        }
        /// <summary>
        /// GetJSONObject
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static JSONObject getJSONObject(this JSONObject json, string name)
        {
            return json.GetJSONObject(name);
        }
        /// <summary>
        /// Keys
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IIterator keys(this JSONObject json)
        {
            return json.Keys();
        }
        /// <summary>
        /// Keys
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int length(this JSONObject json)
        {
            return json.Length();
        }
    }
}