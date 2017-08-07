using System;
using Org.Apache.Http.Message;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.Message.AbstractHttpMessage À©Õ¹
    /// </summary>
    public static class abstractHttpMessage
    {
        /// <summary>
        /// AddHeader
        /// </summary>
        /// <param name="httpMessage"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void addHeader(this AbstractHttpMessage httpMessage, string name, string value)
        {
            httpMessage.AddHeader(name, value);
        }
        /// <summary>
        /// SetHeader
        /// </summary>
        /// <param name="httpMessage"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setHeader(this AbstractHttpMessage httpMessage, string name, string value)
        {
            httpMessage.SetHeader(name, value);
        }
        /// <summary>
        /// RemoveHeaders
        /// </summary>
        /// <param name="httpMessage"></param>
        /// <param name="name"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void removeHeaders(this AbstractHttpMessage httpMessage, string name)
        {
            httpMessage.RemoveHeaders(name);
        }
    }
}