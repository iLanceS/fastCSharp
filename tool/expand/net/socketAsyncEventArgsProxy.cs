#if NOEXPAND
#else
using System;
using System.Net.Sockets;
using System.Reflection;

namespace fastCSharp.net
{
    /// <summary>
    /// 异步回调参数
    /// </summary>
    internal static class socketAsyncEventArgsProxy
    {
        /// <summary>
        /// 获取一个异步回调参数
        /// </summary>
        internal static Func<SocketAsyncEventArgs> Get = (Func<SocketAsyncEventArgs>)Delegate.CreateDelegate(typeof(Func<SocketAsyncEventArgs>), typeof(socketAsyncEventArgs).GetMethod("Get", BindingFlags.NonPublic | BindingFlags.Static));
        /// <summary>
        /// 获取一个异步回调参数
        /// </summary>
        internal static pushPool<SocketAsyncEventArgs> Push = (pushPool<SocketAsyncEventArgs>)Delegate.CreateDelegate(typeof(pushPool<SocketAsyncEventArgs>), typeof(socketAsyncEventArgs).GetMethod("Push", BindingFlags.NonPublic | BindingFlags.Static));
    }
}
#endif
