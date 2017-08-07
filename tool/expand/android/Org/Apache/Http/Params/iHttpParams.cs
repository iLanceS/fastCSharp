using System;
using Org.Apache.Http.Params;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.Params.IHttpParams À©Õ¹
    /// </summary>
    public static class iHttpParams
    {
        /// <summary>
        /// SetParameter
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IHttpParams setParameter(this IHttpParams parameters, string name, Java.Lang.Object value)
        {
            return parameters.SetParameter(name, value);
        }
        /// <summary>
        /// RemoveParameter
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool removeParameter(this IHttpParams parameters, string name)
        {
            return parameters.RemoveParameter(name);
        }
    }
}