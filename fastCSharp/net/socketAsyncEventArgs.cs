using System;
using System.Net.Sockets;

namespace fastCSharp.net
{
    /// <summary>
    /// 异步回调参数
    /// </summary>
    public static class socketAsyncEventArgs
    {
        /// <summary>
        /// 获取一个异步回调参数
        /// </summary>
        /// <returns>异步回调参数</returns>
        internal static SocketAsyncEventArgs Get()
        {
            SocketAsyncEventArgs value = typePool<SocketAsyncEventArgs>.Pop();
            if (value == null)
            {
                value = new SocketAsyncEventArgs();
                value.SocketFlags = System.Net.Sockets.SocketFlags.None;
                value.DisconnectReuseSocket = false;
            }
            return value;
        }
        /// <summary>
        /// 添加异步回调参数
        /// </summary>
        /// <param name="value">异步回调参数</param>
        internal static void Push(ref SocketAsyncEventArgs value)
        {
            value.SetBuffer(null, 0, 0);
            value.UserToken = null;
            value.SocketError = SocketError.Success;
            typePool<SocketAsyncEventArgs>.Push(ref value);
        }
        /// <summary>
        /// 添加异步回调参数
        /// </summary>
        /// <param name="value">异步回调参数</param>
        internal static void PushNotNull(SocketAsyncEventArgs value)
        {
            value.SetBuffer(null, 0, 0);
            value.UserToken = null;
            value.SocketError = SocketError.Success;
            typePool<SocketAsyncEventArgs>.PushNotNull(value);
        }
    }
}
