using System;
using System.Collections.Generic;
using Android.OS;

namespace fastCSharp.android
{
    /// <summary>
    /// Android.OS.BaseBundle 扩展
    /// </summary>
    public static class baseBundle
    {
        /// <summary>
        /// 判断是否存在关键字 ContainsKey
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool containsKey(this BaseBundle bundle, string key)
        {
            return bundle.ContainsKey(key);
        }
        /// <summary>
        /// 获取键值对数量 Size
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int size(this BaseBundle bundle)
        {
            return bundle.Size();
        }
        /// <summary>
        /// 添加字符串键值对 PutString
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void putString(this BaseBundle bundle, string key, string value)
        {
            bundle.PutString(key, value);
        }
        /// <summary>
        /// 根据关键字获取字符串 GetString
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getString(this BaseBundle bundle, string key)
        {
            return bundle.GetString(key);
        }
        /// <summary>
        /// 删除关键字 Remove
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void remove(this BaseBundle bundle, string key)
        {
            bundle.Remove(key);
        }
        /// <summary>
        /// 获取关键字集合 KeySet
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ICollection<string> keySet(this BaseBundle bundle)
        {
            return bundle.KeySet();
        }
        /// <summary>
        /// 根据关键字获取数据 Get
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Object get(this BaseBundle bundle, string key)
        {
            return bundle.Get(key);
        }
        /// <summary>
        /// 根据关键字获取字符串数组 GetStringArray
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string[] getStringArray(this BaseBundle bundle, string key)
        {
            return bundle.GetStringArray(key);
        }
        /// <summary>
        /// 判断是否存在键值对 IsEmpty
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool isEmpty(this BaseBundle bundle)
        {
            return bundle.IsEmpty;
        }
        /// <summary>
        /// GetInt
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int getInt(this BaseBundle bundle, string key, int defaultValue)
        {
            return bundle.GetInt(key, defaultValue);
        }
    }
}