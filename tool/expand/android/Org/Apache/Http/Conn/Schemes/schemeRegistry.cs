using System;
using Org.Apache.Http.Conn.Schemes;

namespace fastCSharp.android
{
    /// <summary>
    /// Org.Apache.Http.Conn.Schemes.SchemeRegistry À©Õ¹
    /// </summary>
    public static class schemeRegistry
    {
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="schemeregistry"></param>
        /// <param name="sch"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Scheme register(this SchemeRegistry schemeregistry, Scheme sch)
        {
            return schemeregistry.Register(sch);
        }

    }
}