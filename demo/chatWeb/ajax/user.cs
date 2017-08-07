using System;
using System.Collections.Generic;
using fastCSharp.code.cSharp;
using fastCSharp.threading;
using System.Threading;

namespace fastCSharp.demo.chatWeb.ajax
{
    /// <summary>
    /// 用户相关ajax调用
    /// </summary>
    [fastCSharp.code.cSharp.ajax(IsPool = true, IsExportTypeScript = true)]
    internal sealed class user : fastCSharp.code.cSharp.ajax.call<user>
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        private sealed class userInfo
        {
            /// <summary>
            /// 最后一次操作时间
            /// </summary>
            public DateTime LastTime = date.NowSecond;
            /// <summary>
            /// 长轮询超时
            /// </summary>
            public DateTime PollTimeout;
            /// <summary>
            /// 长轮询回调委托
            /// </summary>
            public Action<poll.message> OnPoll;
            /// <summary>
            /// 用户聊天消息集合
            /// </summary>
            public list<data.message> Messages = new list<data.message>();
        }
        /// <summary>
        /// 登录用户信息
        /// </summary>
        private static Dictionary<hashString, userInfo> userInfos = dictionary.CreateHashString<userInfo>();
        /// <summary>
        /// 登录用户版本信息
        /// </summary>
        private static int userVersion;
        /// <summary>
        /// 登录用户信息访问锁
        /// </summary>
        private static readonly object userLock = new object();
        /// <summary>
        /// 获取用户更新回调集合
        /// </summary>
        /// <param name="currentUserInfo">当前用户信息</param>
        /// <param name="isUsers">是否返回用户信息集合</param>
        /// <returns>用户更新回调集合</returns>
        private static keyValue<list<Action<poll.message>>, string[]> getOnPolls(userInfo currentUserInfo, bool isUsers)
        {
            list<Action<poll.message>> onPolls = new list<Action<poll.message>>(userInfos.Count);
            foreach (userInfo userInfo in userInfos.Values)
            {
                if (userInfo.OnPoll != null && userInfo != currentUserInfo)
                {
                    onPolls.UnsafeAdd(userInfo.OnPoll);
                    userInfo.OnPoll = null;
                }
            }
            return new keyValue<list<Action<poll.message>>, string[]>(onPolls.Count == 0 ? null : onPolls, isUsers || onPolls.Count != 0 ? userInfos.Keys.getArray(value => value.ToString()) : null);
        }
        /// <summary>
        /// 用户更新回调
        /// </summary>
        /// <param name="onPolls">用户更新回调集合</param>
        private static void onUserChange(keyValue<list<Action<poll.message>>, poll.message> onPolls)
        {
            poll(onPolls.Key, onPolls.Value);
        }
        /// <summary>
        /// 长轮询推送
        /// </summary>
        /// <param name="onPolls">长轮询委托集合</param>
        /// <param name="message">长轮询消息</param>
        private static void poll(list<Action<poll.message>> onPolls, poll.message message)
        {
            foreach (Action<poll.message> onPoll in onPolls)
            {
                try
                {
                    onPoll(message);
                }
                catch { }
            }
        }
        /// <summary>
        /// 取消长轮询回调
        /// </summary>
        /// <param name="onPoll">长轮询回调</param>
        private static void remove(Action<poll.message> onPoll)
        {
            if (onPoll != null)
            {
                try
                {
                    onPoll(null);
                }
                catch { }
            }
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="version">用户版本</param>
        /// <returns>用户集合</returns>
        public string[] Login(string user, ref int version)
        {
            if (user.length() != 0)
            {
                keyValue<list<Action<poll.message>>, string[]> onPolls = default(keyValue<list<Action<poll.message>>, string[]>);
                userInfo userInfo = new userInfo(), removeUserInfo = null;
                Action<poll.message> onPoll = null;
                hashString userHash = user;
                bool isLogin = false;
                Monitor.Enter(userLock);
                try
                {
                    if (userInfos.TryGetValue(userHash, out removeUserInfo))
                    {
                        if (removeUserInfo.LastTime.AddMinutes(2) <= date.NowSecond)
                        {
                            onPoll = removeUserInfo.OnPoll;
                            removeUserInfo.OnPoll = null;

                            isLogin = true;
                            version = ++userVersion;
                            userInfos[userHash] = userInfo;
                            onPolls = getOnPolls(userInfo, true);
                        }
                    }
                    else
                    {
                        isLogin = true;
                        version = ++userVersion;
                        userInfos.Add(userHash, userInfo);
                        onPolls = getOnPolls(userInfo, true);
                    }
                }
                finally { Monitor.Exit(userLock); }
                if (onPolls.Key != null)
                {
                    threadPool.TinyPool.Start(onUserChange, new keyValue<list<Action<poll.message>>, poll.message>(onPolls.Key, new poll.message { UserVersion = version, Users = onPolls.Value }));
                }
                remove(onPoll);
                if (isLogin) return onPolls.Value ?? nullValue<string>.Array;
            }
            return null;
        }
        /// <summary>
        /// 用户注销
        /// </summary>
        /// <param name="user">用户名</param>
        public void Logout(string user)
        {
            if (user.length() != 0)
            {
                RemoveSession();
                int version = 0;
                userInfo userInfo;
                keyValue<list<Action<poll.message>>, string[]> onPolls = default(keyValue<list<Action<poll.message>>, string[]>);
                Action<poll.message> onPoll = null;
                hashString userHash = user;
                Monitor.Enter(userLock);
                try
                {
                    if (userInfos.TryGetValue(userHash, out userInfo))
                    {
                        onPoll = userInfo.OnPoll;
                        userInfos.Remove(userHash);
                        version = ++userVersion;
                        onPolls = getOnPolls(userInfo, false);
                        userInfo.OnPoll = null;
                    }
                }
                finally { Monitor.Exit(userLock); }
                if (onPolls.Key != null)
                {
                    threadPool.TinyPool.Start(onUserChange, new keyValue<list<Action<poll.message>>, poll.message>(onPolls.Key, new poll.message { UserVersion = version, Users = onPolls.Value }));
                }
                remove(onPoll);
            }
        }
        /// <summary>
        /// 消息回调
        /// </summary>
        private struct messagePoll
        {
            /// <summary>
            /// 聊天消息
            /// </summary>
            public data.message Message;
            /// <summary>
            /// 消息回调集合
            /// </summary>
            public list<Action<poll.message>> OnPolls;
            /// <summary>
            /// 添加回调用户
            /// </summary>
            /// <param name="userInfo">用户信息</param>
            public void Add(userInfo userInfo)
            {
                if (userInfo.OnPoll == null) userInfo.Messages.Add(Message);
                else
                {
                    OnPolls.Add(userInfo.OnPoll);
                    userInfo.OnPoll = null;
                }
            }
            /// <summary>
            /// 消息回调推送
            /// </summary>
            public void Poll()
            {
                poll(OnPolls, new poll.message { Messages = new data.message[] { Message } });
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="user">消息发送者</param>
        /// <param name="message">聊天消息</param>
        /// <param name="users">接收用户列表</param>
        public void Send(string user, string message, string[] users)
        {
            if (user.length() != 0)
            {
                messagePoll messagePoll = new messagePoll { Message = new data.message { User = user, Message = fastCSharp.web.formatHtml.Format(message) }, OnPolls = new list<Action<poll.message>>() };
                userInfo currentUserInfo;
                hashString userHash = user;
                Monitor.Enter(userLock);
                try
                {
                    if (userInfos.TryGetValue(userHash, out currentUserInfo))
                    {
                        if (users.length() == 0)
                        {
                            foreach (userInfo userInfo in userInfos.Values)
                            {
                                if (userInfo != currentUserInfo) messagePoll.Add(userInfo);
                            }
                        }
                        else
                        {
                            userInfo userInfo;
                            foreach (string name in users)
                            {
                                if (userInfos.TryGetValue(name, out userInfo)) messagePoll.Add(userInfo);
                            }
                        }
                    }
                }
                finally { Monitor.Exit(userLock); }
                if (messagePoll.OnPolls.Count != 0) threadPool.TinyPool.Start(messagePoll.Poll);
            }
        }
        /// <summary>
        /// 长轮询超时检测
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        private static void pollTimeout(userInfo userInfo)
        {
            if (userInfo.OnPoll != null && userInfo.PollTimeout <= date.NowSecond)
            {
                Action<poll.message> onPoll = null;
                Monitor.Enter(userLock);
                if (userInfo.OnPoll != null && userInfo.PollTimeout <= date.NowSecond)
                {
                    onPoll = userInfo.OnPoll;
                    userInfo.OnPoll = null;
                }
                Monitor.Exit(userLock);
                remove(onPoll);
            }
        }
        /// <summary>
        /// 长轮询回调注册
        /// </summary>
        /// <param name="user">登录用户</param>
        /// <param name="version">登录用户版本信息</param>
        /// <param name="onPoll">长轮询回调委托</param>
        /// <returns>长轮询消息</returns>
        public static poll.message GetMessage(string user, int version, Action<poll.message> onPoll)
        {
            poll.message message = null;
            Action<poll.message> oldOnPoll = null;
            userInfo userInfo = null;
            hashString userHash = user;
            bool isTimeout = false;
            Monitor.Enter(userLock);
            try
            {
                if (userInfos.TryGetValue(userHash, out userInfo))
                {
                    userInfo.LastTime = date.NowSecond;
                    if (version == userVersion)
                    {
                        oldOnPoll = userInfo.OnPoll;
                        if (userInfo.Messages.Count == 0)
                        {
                            userInfo.PollTimeout = date.NowSecond.AddSeconds(60);
                            userInfo.OnPoll = onPoll;
                            isTimeout = true;
                        }
                        else
                        {
                            message = new poll.message { Messages = userInfo.Messages.GetArray() };
                            userInfo.Messages.Clear();
                        }
                    }
                    else message = new poll.message { UserVersion = userVersion, Users = userInfos.Keys.getArray(value => value.ToString()) };
                }
                else message = chatWeb.poll.message.Null;
            }
            finally { Monitor.Exit(userLock); }
            if (oldOnPoll != null) oldOnPoll(null);
            if (isTimeout) threading.timerTask.Default.Add(pollTimeout, userInfo, userInfo.PollTimeout, null);
            return message;
        }
    }
}
