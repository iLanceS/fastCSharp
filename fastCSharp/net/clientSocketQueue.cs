using System;
using System.Net.Sockets;
using System.Net;
using fastCSharp.threading;

namespace fastCSharp.net
{
    /// <summary>
    /// 客户端套接字队列
    /// </summary>
    public class clientSocketQueue : clientQueue<Socket>
    {
        /// <summary>
        /// 客户端空队列
        /// </summary>
        private sealed class nullQueue : clientSocketQueue
        {
            /// <summary>
            /// 添加客户端
            /// </summary>
            /// <param name="socket">套接字</param>
            /// <param name="ipv4">ipv4地址</param>
            /// <param name="ipv6">ipv6地址</param>
            /// <returns>套接字操作类型</returns>
            public override socketType NewClient(Socket socket, ref int ipv4, ref ipv6Hash ipv6)
            {
                ipv4 = 0;
                return socketType.Ipv4;
            }
            /// <summary>
            /// 请求处理结束
            /// </summary>
            /// <param name="ipv4">ipv4地址</param>
            /// <returns>下一个客户端</returns>
            public override Socket End(int ipv4)
            {
                return null;
            }
            /// <summary>
            /// 请求处理结束
            /// </summary>
            /// <param name="ipv6">ipv6地址</param>
            /// <returns>下一个客户端</returns>
            public override Socket End(ipv6Hash ipv6)
            {
                return null;
            }
            /// <summary>
            /// 客户端空队列
            /// </summary>
            private nullQueue() : base(0, 0) { }
            /// <summary>
            /// 客户端空队列
            /// </summary>
            public static readonly nullQueue Default = new nullQueue();
        }
        /// <summary>
        /// 客户端套接字队列
        /// </summary>
        /// <param name="maxActiveCount">每IP最大活动连接数量,等于0表示不限</param>
        /// <param name="maxCount">每IP最大连接数量,等于0表示不限</param>
        private clientSocketQueue(int maxActiveCount, int maxCount) : base(maxActiveCount, maxCount) { }
        /// <summary>
        /// 添加客户端
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="ipv4">ipv4地址</param>
        /// <param name="ipv6">ipv6地址</param>
        /// <returns>套接字操作类型</returns>
        public virtual socketType NewClient(Socket socket, ref int ipv4, ref ipv6Hash ipv6)
        {
            return base.NewClient(socket, socket, ref ipv4, ref ipv6);
        }
        /// <summary>
        /// 客户端套接字队列
        /// </summary>
        /// <param name="maxActiveCount">每IP最大活动连接数量,小于等于0表示不限</param>
        /// <param name="maxCount">每IP最大连接数量,小于等于0表示不限</param>
        public new static clientSocketQueue Create(int maxActiveCount, int maxCount)
        {
            if (maxActiveCount <= 0) return nullQueue.Default;
            return new clientSocketQueue(maxActiveCount, maxCount < maxActiveCount ? maxActiveCount : maxCount);
        }
    }
}
