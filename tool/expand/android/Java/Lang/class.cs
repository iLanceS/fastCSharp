using System;
using Java.Lang;
using Java.Lang.Reflect;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Class À©Õ¹
    /// </summary>
    public static class classExpand
    {
        /// <summary>
        /// GetMethod
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Method getMethod(this Class value, string name, params Class[] parameterTypes)
        {
            return value.GetMethod(name, parameterTypes);
        }
        /// <summary>
        /// GetDeclaredMethods
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Method[] getDeclaredMethods(this Class value)
        {
            return value.GetDeclaredMethods();
        }
        /// <summary>
        /// Name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getName(this Class value)
        {
            return value.Name;
        }
    }
}