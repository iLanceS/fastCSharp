using System;
using Java.Lang.Reflect;

namespace fastCSharp.android
{
    /// <summary>
    /// Java.Lang.Reflect.Method À©Õ¹
    /// </summary>
    public static class method
    {
        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="method"></param>
        /// <param name="receiver"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Object invoke(this Method method, Java.Lang.Object receiver, params Java.Lang.Object[] args)
        {
            return method.Invoke(receiver, args);
        }
        /// <summary>
        /// Name
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string getName(this Method method)
        {
            return method.Name;
        }
        /// <summary>
        /// GetParameterTypes
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Class[] getParameterTypes(this Method method)
        {
            return method.GetParameterTypes();
        }
        /// <summary>
        /// ReturnType
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Java.Lang.Class getReturnType(this Method method)
        {
            return method.ReturnType;
        }
    }
}