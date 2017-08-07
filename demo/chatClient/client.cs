using System;
using System.Threading;
using fastCSharp.code.cSharp;
using fastCSharp.net;
using fastCSharp.net.tcp;

namespace fastCSharp.demo.chatServer
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class client : IDisposable
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private server.tcpClient tcpClient;
        /// <summary>
        /// 用户列表更新保持回调
        /// </summary>
        private commandClient.streamCommandSocket.keepCallback userChangeCallback;
        /// <summary>
        /// 消息更新保持回调
        /// </summary>
        private commandClient.streamCommandSocket.keepCallback messageCallback;
        /// <summary>
        /// 用户名
        /// </summary>
        public string User { get; private set; }
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        private int isDisposed;
        /// <summary>
        /// 是否已经释放资源
        /// </summary>
        public bool IsDisposed
        {
            get { return isDisposed != 0; }
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        private string[] users;
        /// <summary>
        /// 用户列表
        /// </summary>
        public string[] Users
        {
            get { return users.removeFirst(User); }
        }
        /// <summary>
        /// 用户列表更新
        /// </summary>
        public event Action<string[]> OnUserChanged;
        /// <summary>
        /// 消息更新
        /// </summary>
        public event Action<server.message> OnMessage;
        /// <summary>
        /// 释放资源
        /// </summary>
        public event Action OnDisposed;
        /// <summary>
        /// 客户端
        /// </summary>
        /// <param name="user">用户名</param>
        public client(string user)
        {
            tcpClient = new server.tcpClient();
            if (tcpClient.login(user).Value)
            {
                User = user;
                userChangeCallback = tcpClient.getUsers(userChange);
                messageCallback = tcpClient.receive(receive);
            }
            else Dispose();
        }
        /// <summary>
        /// 用户列表更新
        /// </summary>
        /// <param name="usersVerison">用户列表与版本信息</param>
        private void userChange(returnValue<string[]> usersVerison)
        {
            if (usersVerison.Type == returnValue.type.Success)
            {
                users = usersVerison.Value;
                if (OnUserChanged != null) OnUserChanged(Users);
            }
            else Dispose();
        }
        /// <summary>
        /// 消息更新
        /// </summary>
        /// <param name="messages">消息列表</param>
        private void receive(returnValue<server.message> messages)
        {
            if (messages.Type == returnValue.type.Success)
            {
                if (OnMessage != null) OnMessage(messages.Value);
            }
            else Dispose();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="users">接收用户列表,null表示所有用户</param>
        public void Send(string message, string[] users = null)
        {
            tcpClient.send(message, users);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref isDisposed) == 1)
            {
                try
                {
                    using (tcpClient)
                    {
                        pub.Dispose(ref userChangeCallback);
                        pub.Dispose(ref messageCallback);
                        if (User != null) tcpClient.logout();
                    }
                    tcpClient = null;
                }
                finally
                {
                    if (OnDisposed != null) OnDisposed();
                }
            }
        }
    }
}
