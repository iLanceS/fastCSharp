using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace fastCSharp.web
{
    /// <summary>
    /// HTTP参数及其相关操作
    /// </summary>
    public static class http
    {
        /// <summary>
        /// 查询模式类别
        /// </summary>
        public enum methodType : byte
        {
            /// <summary>
            /// 未知查询模式类别
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// 请求获取Request-URI所标识的资源
            /// </summary>
            GET,
            /// <summary>
            /// 在Request-URI所标识的资源后附加新的数据
            /// </summary>
            POST,
            /// <summary>
            /// 请求获取由Request-URI所标识的资源的响应消息报头
            /// </summary>
            HEAD,
            /// <summary>
            /// 请求服务器存储一个资源，并用Request-URI作为其标识
            /// </summary>
            PUT,
            /// <summary>
            /// 请求服务器删除Request-URI所标识的资源
            /// </summary>
            DELETE,
            /// <summary>
            /// 请求服务器回送收到的请求信息，主要用于测试或诊断
            /// </summary>
            TRACE,
            /// <summary>
            /// 保留将来使用
            /// </summary>
            CONNECT,
            /// <summary>
            /// 请求查询服务器的性能，或者查询与资源相关的选项和需求
            /// </summary>
            OPTIONS
        }
        /// <summary>
        /// 查询模式类型集合
        /// </summary>
        private static pointer uniqueTypes;
        /// <summary>
        /// 查询模式字节转枚举
        /// </summary>
        /// <param name="method">查询模式</param>
        /// <returns>查询模式枚举</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        internal static unsafe methodType GetMethod(byte* method)
        {
            uint code = *(uint*)method;
            return (methodType)uniqueTypes.Byte[((code >> 12) ^ code) & ((1U << 4) - 1)];
        }

        unsafe static http()
        {
            uniqueTypes = unmanaged.GetStatic(1 << 4, true);
            uint code;
            byte* methodBufferFixed = (byte*)&code;
            foreach (methodType method in System.Enum.GetValues(typeof(fastCSharp.web.http.methodType)))
            {
                if (method != methodType.Unknown)
                {
                    string methodString = method.ToString();
                    fixed (char* methodFixed = methodString)
                    {
                        byte* write = methodBufferFixed, end = methodBufferFixed;
                        if (methodString.Length >= sizeof(int)) end += sizeof(int);
                        else
                        {
                            code = 0x20202020U;
                            end += methodString.Length;
                        }
                        for (char* read = methodFixed; write != end; *write++ = (byte)*read++) ;
                        uniqueTypes.Byte[((code >> 12) ^ code) & ((1U << 4) - 1)] = (byte)method;
                    }
                }
            }
        }
    }
}
