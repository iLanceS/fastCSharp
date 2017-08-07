using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using fastCSharp.demo.chatServer;
using System.Threading;

namespace fastCSharp.demo.chatClient
{
    internal partial class form : Form
    {
        /// <summary>
        /// UI线程上下文
        /// </summary>
        private readonly SynchronizationContext context;
        /// <summary>
        /// 客户端
        /// </summary>
        private client client;
        /// <summary>
        /// 当前用户列表
        /// </summary>
        private string[] currentUsers;
        /// <summary>
        /// 发送用户列表
        /// </summary>
        private list<string> sendUsers = new list<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">默认用户名</param>
        public form(string userName)
        {
            InitializeComponent();

            context = SynchronizationContext.Current;

            if (userName == null)
            {
                newMessage(new server.message { Time = date.NowSecond, User = "fastCSharp", Message = "请使用 fastCSharp.demo.chatServer.exe 启动示例" });
            }
            else
            {
                userNameTextBox.Text = userName;
                loginButton_Click(null, null);
                sendTextBox.Text = "大家好，我是 " + userName;
                sendButton_Click(null, null);
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onFormClose(object sender, FormClosedEventArgs e)
        {
            pub.Dispose(ref client);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            string user = userNameTextBox.Text;
            if (user.length() != 0)
            {
                try
                {
                    if ((client = new client(user)).IsDisposed) MessageBox.Show("登录失败");
                    else
                    {
                        userNameTextBox.Enabled = loginButton.Enabled = false;
                        client.OnUserChanged += onUserChanged;
                        client.OnMessage += onMessage;
                        client.OnDisposed += onCloseClient;
                        sendTextBox.Enabled = sendButton.Enabled = logoutButton.Enabled = true;
                        setUsers(client.Users);
                    }
                }
                catch { }
            }
        }
        /// <summary>
        /// 用户列表更新
        /// </summary>
        /// <param name="value">用户列表</param>
        private void setUsers(object value)
        {
            currentUsers = (string[])value;
            if (currentUsers.Length == 0)
            {
                userListBox.Items.Clear();
                allUserButton.Enabled = changeUserButton.Enabled = clearUserButton.Enabled = userListBox.Enabled = false;
            }
            else
            {
                allUserButton.Enabled = changeUserButton.Enabled = clearUserButton.Enabled = userListBox.Enabled = true;
                userListBox.Items.Clear();
                userListBox.Items.AddRange(currentUsers);
            }
        }
        /// <summary>
        /// 用户列表更新
        /// </summary>
        /// <param name="users">用户列表</param>
        private void onUserChanged(string[] users)
        {
            context.Post(setUsers, users);
        }
        /// <summary>
        /// 消息更新
        /// </summary>
        /// <param name="value">消息列表</param>
        private void setMessage(object value)
        {
            newMessage((server.message)value);
        }
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="message">消息</param>
        private void newMessage(server.message message)
        {
            using (charStream charStream = new charStream())
            {
                charStream.Write(@"
");
                charStream.Write(message.User);
                charStream.Write(" ");
                charStream.Write(message.Time.toString());
                charStream.Write(@" :
");
                charStream.Write(message.Message);
                messageTextBox.AppendText(charStream.ToString());
            }
            messageTextBox.ScrollToCaret();

        }
        /// <summary>
        /// 消息更新
        /// </summary>
        /// <param name="messages">消息列表</param>
        private void onMessage(server.message message)
        {
            context.Post(setMessage, message);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="value"></param>
        private void onCloseClient(object value)
        {
            userListBox.Items.Clear();
            messageTextBox.Text = string.Empty;
            allUserButton.Enabled = changeUserButton.Enabled = clearUserButton.Enabled = userListBox.Enabled = false;
            sendTextBox.Enabled = sendButton.Enabled = logoutButton.Enabled = false;
            userNameTextBox.Enabled = loginButton.Enabled = true;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        private void onCloseClient()
        {
            context.Post(onCloseClient, null);
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logoutButton_Click(object sender, EventArgs e)
        {
            pub.Dispose(ref client);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendButton_Click(object sender, EventArgs e)
        {
            sendUsers.Empty();
            for (int index = currentUsers.Length; index != 0; )
            {
                if (userListBox.GetItemChecked(--index)) sendUsers.Add(currentUsers[index]);
            }
            try
            {
                string message = sendTextBox.Text;
                client.Send(message, sendUsers.Count == 0 ? null : sendUsers.toArray());
                newMessage(new server.message { User = client.User, Time = date.NowSecond, Message = message });
                sendTextBox.Text = string.Empty;
            }
            catch
            {
                MessageBox.Show("发送失败");
            }
        }
        /// <summary>
        /// 用户全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allUserButton_Click(object sender, EventArgs e)
        {
            for (int index = currentUsers.Length; index != 0; userListBox.SetItemChecked(--index, true)) ;
        }
        /// <summary>
        /// 反选用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeUserButton_Click(object sender, EventArgs e)
        {
            for (int index = currentUsers.Length; --index >= 0; userListBox.SetItemChecked(index, !userListBox.GetItemChecked(index))) ;
        }
        /// <summary>
        /// 清除用户选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearUserButton_Click(object sender, EventArgs e)
        {
            for (int index = currentUsers.Length; index != 0; userListBox.SetItemChecked(--index, false)) ;
        }
    }
}
