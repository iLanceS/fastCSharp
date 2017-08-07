using System;
using fastCSharp.code.cSharp;
using System.Threading;
using fastCSharp.io;
using fastCSharp.net;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
    /// <summary>
    /// 负载均衡测试web调用
    /// </summary>
    [fastCSharp.code.cSharp.webCall(IsOnlyPost = false, IsPool = true)]
    internal sealed class webCall : fastCSharp.code.cSharp.webCall.call<webCall>
    {
        /// <summary>
        /// 简单连接测试
        /// </summary>
        //[fastCSharp.code.cSharp.webCall]
        public void Check()
        {
            createResponse();
            try
            {
                Response.UnsafeBodyStream.Write('t' + ('r' << 8) + ('u' << 16) + ('e' << 24));
            }
            finally { responseEnd(); }
        }
        /// <summary>
        /// 负载均衡测试调用
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        [fastCSharp.code.cSharp.webCall]
        public void Add(int left, int right)
        {
#if ONLYWEB
            createResponse();
            try
            {
                response((left + right).toString());
            }
            finally { responseEnd(); }
#else
            returnValue<int> value = loadBalancingTcpCommandWeb.loadBalancing.add(left, right);
            if (value.Type == returnValue.type.Success)
            {
                createResponse();
                try
                {
                    response(value.Value.toString());
                }
                finally { responseEnd(); }
            }
            else
            {
                serverError500();
                Console.WriteLine("TCP Error");
            }
#endif
        }
        /// <summary>
        /// 负载均衡测试调用
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.webCall]
        public void Xor(int left, int right)
        {
#if ONLYWEB
                    createResponse();
                    try
                    {
                        response((left ^ right).toString());
                    }
                    finally { responseEnd(); }
#else
            loadBalancingTcpCommandWeb.loadBalancing.xor(left, right, value =>
            {
                if (value.Type == returnValue.type.Success)
                {
                    createResponse();
                    try
                    {
                        response(value.Value.toString());
                    }
                    finally { responseEnd(); }
                }
                else
                {
                    serverError500();
                    Console.WriteLine("TCP Error");
                }
            });
#endif
        }
    }
}
