using System;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using fastCSharp.threading;
using fastCSharp.net;

namespace fastCSharp.demo.chatServer
{
    /// <summary>
    /// 服务端
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(IsIdentityCommand = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    public partial class server
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        private sealed class userInfo
        {
            /// <summary>
            /// 用户名
            /// </summary>
            public string User;
            /// <summary>
            /// 获取用户列表委托
            /// </summary>
            public Func<returnValue<string[]>, bool> OnUserChanged;
            /// <summary>
            /// 获取消息委托
            /// </summary>
            public Func<returnValue<message>, bool> OnMessage;
        }
        /// <summary>
        /// 用户集合
        /// </summary>
        private Dictionary<hashString, userInfo> users = dictionary.CreateHashString<userInfo>();
        /// <summary>
        /// 用户集合访问锁
        /// </summary>
        private readonly object userLock = new object();
        /// <summary>
        /// 用户版本
        /// </summary>
        private int userVersion;
        /// <summary>
        /// 用户登陆
        /// </summary>
        public event Action<string> OnLogin;
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="user"></param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private bool login(fastCSharp.net.tcp.commandServer.socket client, string user)
        {
            if (user.length() != 0)
            {
                hashString userHash = user;
                bool isUser = false;
                Monitor.Enter(userLock);
                try
                {
                    if (!users.ContainsKey(userHash))
                    {
                        users.Add(userHash, new userInfo { User = user });
                        client.ClientUserInfo = user;
                        ++userVersion;
                        isUser = true;
                    }
                }
                finally { Monitor.Exit(userLock); }
                if (isUser)
                {
                    if (userChangeHandle == null) userChangeHandle = userChange;
                    task.Tiny.Add(userChangeHandle, user);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 用户退出
        /// </summary>
        public event Action<string> OnLogout;
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="client"></param>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void logout(fastCSharp.net.tcp.commandServer.socket client)
        {
            string user = (string)client.ClientUserInfo;
            userInfo userInfo;
            hashString userHash = user;
            Monitor.Enter(userLock);
            try
            {
                if (users.TryGetValue(userHash, out userInfo)) users.Remove(userHash);
                ++userVersion;
            }
            finally { Monitor.Exit(userLock); }
            if (userInfo != null)
            {
                if (userChangeHandle == null) userChangeHandle = userChange;
                task.Tiny.Add(userChangeHandle, null);
                if (OnLogout != null) task.Tiny.Add(OnLogout, user);
            }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="user">用户名</param>
        private void logout(string user)
        {
            userInfo userInfo;
            hashString userHash = user;
            Monitor.Enter(userLock);
            try
            {
                if (users.TryGetValue(userHash, out userInfo)) users.Remove(userHash);
                ++userVersion;
            }
            finally { Monitor.Exit(userLock); }
            if (userInfo != null)
            {
                if (userChangeHandle == null) userChangeHandle = userChange;
                task.Tiny.Add(userChangeHandle, null);
                if (OnLogout != null) task.Tiny.Add(OnLogout, user);
            }
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="client"></param>
        /// <param name="onUserChanged"></param>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void getUsers(fastCSharp.net.tcp.commandServer.socket client, Func<returnValue<string[]>, bool> onUserChanged)
        {
            string[] users = null;
            hashString user = (string)client.ClientUserInfo;
            Monitor.Enter(userLock);
            try
            {
                userInfo userInfo;
                if (this.users.TryGetValue(user, out userInfo))
                {
                    userInfo.OnUserChanged = onUserChanged;
                    users = this.users.Keys.getArray(value => value.ToString());
                }
            }
            finally
            {
                Monitor.Exit(userLock);
                onUserChanged(users);
            }
        }
        /// <summary>
        /// 获取用户列表委托集合
        /// </summary>
        private list<userInfo> onUserChangeds = new list<userInfo>();
        /// <summary>
        /// 待删除用户
        /// </summary>
        private list<string> removeUsers = new list<string>();
        /// <summary>
        /// 获取用户列表委托集合访问锁
        /// </summary>
        private int onUserChangedLock;
        /// <summary>
        /// 用户列表更新
        /// </summary>
        private Action<string> userChangeHandle;
        /// <summary>
        /// 用户列表更新
        /// </summary>
        /// <param name="user">新增用户名</param>
        private void userChange(string user)
        {
            string[] users;
            int userVersion = int.MinValue;
            while (userVersion != this.userVersion && Interlocked.CompareExchange(ref onUserChangedLock, 1, 0) == 0)
            {
                try
                {
                    Monitor.Enter(userLock);
                    try
                    {
                        users = this.users.Keys.getArray(value => value.ToString());
                        userVersion = this.userVersion;
                        foreach (userInfo userInfo in this.users.Values)
                        {
                            if ((userInfo.User != user || userVersion != int.MinValue) && userInfo.OnUserChanged != null) onUserChangeds.Add(userInfo);
                        }
                    }
                    finally { Monitor.Exit(userLock); }
                    while (userVersion == this.userVersion && onUserChangeds.Count != 0)
                    {
                        try
                        {
                            userInfo userInfo = onUserChangeds.Pop();
                            if (!userInfo.OnUserChanged(users)) removeUsers.Add(userInfo.User);
                        }
                        catch { }
                    }
                    onUserChangeds.Empty();
                }
                finally { onUserChangedLock = 0; }
                if (removeUsers.Count != 0)
                {
                    foreach (string removeUser in removeUsers) logout(user);
                    removeUsers.Empty();
                }
            }
            if (user != null && OnLogin != null) OnLogin(user);
        }
        /// <summary>
        /// 消息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        public struct message
        {
            /// <summary>
            /// 发送者
            /// </summary>
            public string User;
            /// <summary>
            /// 发送时间
            /// </summary>
            public DateTime Time;
            /// <summary>
            /// 发送内容
            /// </summary>
            public string Message;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public event Action<message> OnMessage;
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <param name="users">接收用户列表,null表示向所有用户发送消息</param>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void send(fastCSharp.net.tcp.commandServer.socket client, string message, string[] users)
        {
            list<userInfo> userInfos = new list<userInfo>(this.users.Count);
            string user = (string)client.ClientUserInfo;
            hashString userHash = user;
            Monitor.Enter(userLock);
            try
            {
                if (this.users.ContainsKey(userHash))
                {
                    if (users == null)
                    {
                        foreach (userInfo userInfo in this.users.Values)
                        {
                            if (userInfo.User != user && userInfo.OnMessage != null) userInfos.Add(userInfo);
                        }
                    }
                    else
                    {
                        userInfo userInfo;
                        foreach (string receiveUser in users)
                        {
                            if (receiveUser != user && this.users.TryGetValue(receiveUser, out userInfo))
                            {
                                if (userInfo.OnMessage != null) userInfos.Add(userInfo);
                            }
                        }
                    }
                }
            }
            finally { Monitor.Exit(userLock); }
            if (OnMessage != null) OnMessage(new message { User = user, Time = date.NowSecond, Message = message });
            if (userInfos.Count != 0)
            {
                if (newMessageHandle == null) newMessageHandle = newMessage;
                task.Tiny.Add(newMessageHandle, new messageUsers { Users = userInfos, Message = new message { User = user, Time = date.NowSecond, Message = message } });
            }
        }
        /// <summary>
        /// 消息接收用户
        /// </summary>
        private struct messageUsers
        {
            /// <summary>
            /// 用户集合
            /// </summary>
            public list<userInfo> Users;
            /// <summary>
            /// 消息
            /// </summary>
            public message Message;
        }
        /// <summary>
        /// 消息更新
        /// </summary>
        private Action<messageUsers> newMessageHandle;
        /// <summary>
        /// 消息更新
        /// </summary>
        /// <param name="messageUsers">消息接收用户</param>
        private void newMessage(messageUsers messageUsers)
        {
            foreach (userInfo user in messageUsers.Users)
            {
                int isUser = 0;
                try
                {
                    if (user.OnMessage(messageUsers.Message)) isUser = 1; 
                }
                catch { }
                if (isUser == 0) logout(user.User);
            }
        }
        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="onMessage"></param>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void receive(fastCSharp.net.tcp.commandServer.socket client, Func<returnValue<message>, bool> onMessage)
        {
            userInfo userInfo;
            hashString user = (string)client.ClientUserInfo;
            Monitor.Enter(userLock);
            try
            {
                if (users.TryGetValue(user, out userInfo)) userInfo.OnMessage = onMessage;
            }
            finally { Monitor.Exit(userLock); }
        }

    }
}
