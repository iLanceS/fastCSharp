using System;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Org.Apache.Http.Conn;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Params;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.Impl.Client.AbstractHttpClient À©Õ¹
    /// </summary>
    public static class abstractHttpClient
    {
        /// <summary>
        /// Params
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IHttpParams getParams(this AbstractHttpClient httpClient)
        {
            return httpClient.Params;
        }
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IHttpResponse execute(this AbstractHttpClient httpClient, IHttpUriRequest request)
        {
            return httpClient.Execute(request);
        }
        /// <summary>
        /// setKeepAliveStrategy
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="keepAliveStrategy"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void setKeepAliveStrategy(this AbstractHttpClient httpClient, IConnectionKeepAliveStrategy keepAliveStrategy)
        {
            httpClient.setKeepAliveStrategy(keepAliveStrategy);
        }


    }
}