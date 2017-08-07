using System;
using fastCSharp.code.cSharp;
using System.Threading;
using fastCSharp.net;

namespace fastCSharp.demo.loadBalancingTcpCommandWeb
{
    /// <summary>
    /// 负载均衡测试web视图调用
    /// </summary>
    [fastCSharp.code.cSharp.webView(IsPage = true, IsPool = true, IsReferer = false)]
    internal sealed partial class webView : fastCSharp.code.cSharp.webView.view<webView>
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public int Return;
        /// <summary>
        /// 负载均衡测试web视图调用
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="isAsynchronous"></param>
        /// <returns></returns>
        private bool loadView(int left, int right, bool isAsynchronous)
        {
#if ONLYWEB
            if (isAsynchronous)
            {
                setAsynchronous();
                Return = left ^ right;
                callback();
            }
            else Return = left + right;
            return true;
#else
            if (isAsynchronous)
            {
                setAsynchronous();
                loadBalancing.xor(left, right, value =>
                {
                    if (value.Type == returnValue.type.Success)
                    {
                        Return = value.Value;
                        callback();
                    }
                    else
                    {
                        serverError500();
                        Console.WriteLine("TCP Error");
                    }
                });
                return true;
            }
            else
            {
                returnValue<int> value = loadBalancing.add(left, right);
                if (value.Type == returnValue.type.Success)
                {
                    Return = value.Value;
                    return true;
                }
                else Console.WriteLine("TCP Error");
            }
            return false;
#endif
        }
    }
}
