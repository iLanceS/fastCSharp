using System;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.Client.Methods.HttpEntityEnclosingRequestBase À©Õ¹
    /// </summary>
    public static class httpEntityEnclosingRequestBase
    {
        /// <summary>
        /// Entity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setEntity(this HttpEntityEnclosingRequestBase request, IHttpEntity entity)
        {
            request.Entity = entity;
        }
    }
}