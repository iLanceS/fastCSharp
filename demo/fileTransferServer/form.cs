using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using fastCSharp.code;

namespace fastCSharp.demo.fileTransferServer
{
    public partial class form : Form
    {
        /// <summary>
        /// UI线程上下文
        /// </summary>
        private readonly SynchronizationContext context;
#if NotFastCSharpCode
#else
        /// <summary>
        /// 文件传输服务端
        /// </summary>
        private server.tcpServer server;
#endif
        public form()
        {
            InitializeComponent();

            context = SynchronizationContext.Current;

            user.Table.Cache.WaitLoad();
            user.Table.Cache.OnInserted += onUserChange;
            user.Table.Cache.OnDeleted += onUserChange;
            user.Table.Cache.OnUpdated += onUserChange;
            if (user.Table.Cache.Count != 0) onUserChange((user)null);

            fastCSharp.threading.threadPool.TinyPool.Start(start);
        }
        /// <summary>
        /// 用户更新事件
        /// </summary>
        /// <param name="value"></param>
        private void onUserChange(object value)
        {
            string name = userComboBox.Text.Trim();
            user[] users = (user[])value;
            userComboBox.Items.Clear();
            userComboBox.Items.AddRange(users);
            user user = user.Table.Cache.Get(name);
            if (user != null) setUser(user);
        }
        /// <summary>
        /// 修改用户名
        /// </summary>
        private void changeUser()
        {
            string name = userComboBox.Text.Trim();
            user user = user.Table.Cache.Get(name);
            if (user == null) pathCheckedListBox.Items.Clear();
            else setUser(user);
        }
        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeUser(object sender, EventArgs e)
        {
            changeUser();
        }
        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeUser(object sender, KeyPressEventArgs e)
        {
            changeUser();
        }
        /// <summary>
        /// 设置当前用户
        /// </summary>
        /// <param name="user"></param>
        private void setUser(user user)
        {
            passwordTextBox.Text = user.Password;
            setPath(user.Name);
        }
        /// <summary>
        /// 选择用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectUser(object sender, EventArgs e)
        {
            object value = userComboBox.SelectedItem;
            if (value != null) setUser((user)value);
        }
        /// <summary>
        /// 用户更新事件
        /// </summary>
        /// <param name="value"></param>
        private void onUserChange(user value)
        {
            context.Post(onUserChange, user.Table.Cache.GetArray());
        }
        /// <summary>
        /// 用户更新事件
        /// </summary>
        /// <param name="value"></param>
        private void onUserChange(user value, user oldValue, memberMap memberMap)
        {
            context.Post(onUserChange, user.Table.Cache.GetArray());
        }
        /// <summary>
        /// 重置路径列表
        /// </summary>
        /// <param name="userName"></param>
        private void setPath(string userName)
        {
            permission[] permissions = permission.UserCache.GetCache(userName).getArray();
            if (permissions.Length == 0) pathCheckedListBox.Items.Clear();
            else
            {
                HashSet<hashString> paths = pathCheckedListBox.CheckedItems.toGeneric<permission>().getHash(value => (hashString)value.Path);
                permissions = permissions.sort((left, right) => left.Path.CompareTo(right.Path));
                pathCheckedListBox.Items.Clear();
                pathCheckedListBox.Items.AddRange(permissions);
                if (paths.count() != 0)
                {
                    int index = 0;
                    foreach (permission value in permissions)
                    {
                        if (paths.Contains(value.Path)) pathCheckedListBox.SetItemChecked(index, true);
                        ++index;
                    }
                }
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close(object sender, FormClosedEventArgs e)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            pub.Dispose(ref server);
            user.Table.Dispose();
            permission.Table.Dispose();
#endif
        }
        /// <summary>
        /// 选择路径
        /// </summary>
        /// <param name="textBox"></param>
        private void selectPath(TextBox textBox)
        {
            string path = textBox.Text;
            if (path.Length != 0 && Directory.Exists(path)) pathBrowserDialog.SelectedPath = path;
            pathBrowserDialog.ShowDialog();
            path = pathBrowserDialog.SelectedPath;
            if (path.length() != 0) textBox.Text = path.toLower().pathSuffix();
        }
        /// <summary>
        /// 选择路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathButton_Click(object sender, EventArgs e)
        {
            selectPath(pathTextBox);
        }
        /// <summary>
        /// 选择备份路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backupPathButton_Click(object sender, EventArgs e)
        {
            selectPath(backupPathTextBox);
        }
        /// <summary>
        /// 路径全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allPathButton_Click(object sender, EventArgs e)
        {
            for (int index = pathCheckedListBox.Items.Count; index != 0; pathCheckedListBox.SetItemChecked(--index, true)) ;
        }
        /// <summary>
        /// 路径反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePathButton_Click(object sender, EventArgs e)
        {
            for (int index = pathCheckedListBox.Items.Count; --index >= 0; pathCheckedListBox.SetItemChecked(index, !pathCheckedListBox.GetItemChecked(index))) ;
        }
        /// <summary>
        /// 清空路径选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearPathButton_Click(object sender, EventArgs e)
        {
            for (int index = pathCheckedListBox.Items.Count; index != 0; pathCheckedListBox.SetItemChecked(--index, false)) ;
        }
        /// <summary>
        /// 删除用户路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deletePathButton_Click(object sender, EventArgs e)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            string userName = userComboBox.Text.Trim();
            user user = user.Table.Cache.Get(userName);
            if (user != null)
            {
                foreach (permission permission in pathCheckedListBox.CheckedItems)
                {
                    permission.Table.Delete(new permission.primaryKey { UserName = userName, Path = permission.Path });
                }
                setPath(userName);
            }
#endif
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteUserButton_Click(object sender, EventArgs e)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            string userName = userComboBox.Text.Trim();
            user user = user.Table.Cache.Get(userName);
            if (user != null)
            {
                foreach (permission value in permission.UserCache.GetCache(userName)) permission.Table.Delete(value);
                (user.Table.Delete(userName)).PermissionCache = null;
            }
#endif
        }
        /// <summary>
        /// 反射模式成员选择
        /// </summary>
        private static readonly memberMap<user> updatePasswordMember = user.Table.CreateMemberMap().Append(value => value.Password);
#if NotFastCSharpCode
#else
        /// <summary>
        /// 反射模式成员选择
        /// </summary>
        private static readonly memberMap<permission> updatePermissionMember = permission.Table.CreateMemberMap().Append(value => value.BackupPath).Append(value => value.Type);
#endif
        /// <summary>
        /// 修改或者添加用户权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void permissionButton_Click(object sender, EventArgs e)
        {
            string userName = userComboBox.Text.Trim();
            if (userName.Length != 0)
            {
                string password = passwordTextBox.Text.Trim();
                user user = user.Table.Cache.Get(userName);
                if (user == null)
                {
                    if (password.Length != 0)
                    {
                        user = user.Table.Insert(new user { Name = userName, Password = password });
                    }
                }
                else if (user.Password != password && password.Length != 0)
                {
                    user = user.Table.Update(new user { Name = userName, Password = password }, updatePasswordMember);
                }
                if (user != null)
                {
                    string path = pathTextBox.Text.Trim(), backupPath = backupPathTextBox.Text.Trim();
                    if (path.Length != 0)
                    {
#if NotFastCSharpCode
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                        try
                        {
                            path = new DirectoryInfo(path).fullName().toLower();
                            if (backupPath.Length != 0) backupPath = new DirectoryInfo(backupPath).fullName().toLower();
                            permissionType permissionType = permissionType.List;
                            if (readPermissionCheckBox.Checked) permissionType |= permissionType.Read;
                            if (writePermissionCheckBox.Checked) permissionType |= permissionType.Write;
                            permission permissionValue = permission.Table.Cache.Get(new permission.primaryKey { UserName = userName, Path = path });
                            if (permissionValue == null)
                            {
                                permission.Table.Insert(new permission { UserName = userName, Path = path, BackupPath = backupPath, Type = permissionType });
                                setPath(userName);
                            }
                            else if (permissionValue.Type != permissionType || permissionValue.BackupPath != backupPath)
                            {
                                permission.Table.Update(new permission { UserName = userName, Path = path, BackupPath = backupPath, Type = permissionType }, updatePermissionMember);
                                setPath(userName);
                            }
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show(error.ToString());
                        }
#endif
                    }
                }
            }
        }
        /// <summary>
        /// 路径单选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkPath(object sender, ItemCheckEventArgs e)
        {
            permission value = (permission)pathCheckedListBox.Items[e.Index];
            pathTextBox.Text = value.Path;
            readPermissionCheckBox.Checked = (value.Type & permissionType.Read) != permissionType.None;
            writePermissionCheckBox.Checked = (value.Type & permissionType.Write) != permissionType.None;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        private void start()
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            try
            {
                server = new server.tcpServer();
                if (server.Start())
                {
                    context.Post(onStart, null);
                    return;
                }
            }
            catch (Exception error)
            {
                log.Error.Add(error, null, false);
            }
            pub.Dispose(ref server);
            fastCSharp.threading.timerTask.Default.Add(start, date.NowSecond.AddSeconds(10));
#endif
        }
        /// <summary>
        /// 服务启动处理
        /// </summary>
        /// <param name="_"></param>
        private void onStart(object _)
        {
            stopButton.Enabled = true;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            fastCSharp.threading.threadPool.TinyPool.Start(start);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            stopButton.Enabled = false;
            pub.Dispose(ref server);
            startButton.Enabled = true;
#endif
        }
    }
}
