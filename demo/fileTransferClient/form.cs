using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using fastCSharp.demo.fileTransferServer;
using System.IO;
using fastCSharp.threading;
using fastCSharp.code;
using fastCSharp.code.cSharp;

namespace fastCSharp.demo.fileTransferClient
{
    public partial class form : Form
    {
        /// <summary>
        /// 历史路径
        /// </summary>
        private sealed class historyPath
        {
            /// <summary>
            /// 历史路径记录
            /// </summary>
            public list<lowerName> History = new list<lowerName>();
            /// <summary>
            /// 历史路径记录索引
            /// </summary>
            public int CurrentIndex;
            /// <summary>
            /// 获取或设置 上一个历史路径
            /// </summary>
            public lowerName Previous
            {
                get
                {
                    return CurrentIndex > 1 ? History[CurrentIndex - 2] : default(lowerName);
                }
                set
                {
                    if (CurrentIndex > 1)
                    {
                        History[CurrentIndex - 2] = value;
                        --CurrentIndex;
                    }
                }
            }
            /// <summary>
            /// 获取或设置 下一个历史路径
            /// </summary>
            public lowerName Next
            {
                get
                {
                    return CurrentIndex == History.Count ? default(lowerName) : History[CurrentIndex];
                }
                set
                {
                    if (CurrentIndex != History.Count) History[CurrentIndex++] = value;
                }
            }
            /// <summary>
            /// 设置历史路径
            /// </summary>
            /// <param name="path">路径</param>
            public void Set(lowerName path)
            {
                if (CurrentIndex == 0 || History[CurrentIndex - 1].LowerName != path.LowerName)
                {
                    if (CurrentIndex == History.Count)
                    {
                        History.Add(path);
                        ++CurrentIndex;
                    }
                    else History[CurrentIndex++] = path;
                }
            }
        }
        /// <summary>
        /// 用户视图数据
        /// </summary>
        private sealed class userView : IDisposable, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<server.tcpClient>
        {
            /// <summary>
            /// 服务器端列表名称
            /// </summary>
            public sealed class serverListName
            {
                /// <summary>
                /// 服务器端路径
                /// </summary>
                public serverPath ServerPath;
                /// <summary>
                /// 列表名称
                /// </summary>
                public server.listName ListName;
                /// <summary>
                /// 获取文件扩展名
                /// </summary>
                public unsafe subString ExtensionName
                {
                    get
                    {
                        fixed (char* nameFixed = ListName.LowerName)
                        {
                            for (char* end = nameFixed + ListName.LowerName.Length; end != nameFixed; )
                            {
                                if (*--end == '.') return new subString(ListName.LowerName, (int)(end - nameFixed) + 1);
                            }
                        }
                        return new subString(string.Empty);
                    }
                }
                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    return toString(ListName);
                }
            }
            /// <summary>
            /// 服务器端路径
            /// </summary>
            public sealed class serverPath
            {
                /// <summary>
                /// 服务器端权限路径
                /// </summary>
                public serverPermission ServerPermission;
                /// <summary>
                /// 路径名称
                /// </summary>
                public lowerName Path;
                /// <summary>
                /// 当前路径名称
                /// </summary>
                public unsafe string LastPath
                {
                    get
                    {
                        if (Path.LowerName.Length != 0)
                        {
                            char directorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
                            fixed (char* pathFixed = Path.LowerName)
                            {
                                for (char* end = pathFixed + Path.LowerName.Length - 1; end != pathFixed; )
                                {
                                    if (*--end == directorySeparatorChar)
                                    {
                                        int index = (int)(end - pathFixed) + 1;
                                        return Path.LowerName.Substring(index, Path.LowerName.Length - index);
                                    }
                                }
                            }
                            return Path.LowerName;
                        }
                        return null;
                    }
                }
                /// <summary>
                /// 父级路径
                /// </summary>
                public unsafe string ParentPath
                {
                    get
                    {
                        if (Path.Name.Length != 0)
                        {
                            char directorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
                            fixed (char* pathFixed = Path.Name)
                            {
                                for (char* end = pathFixed + Path.Name.Length - 1; end != pathFixed; )
                                {
                                    if (*--end == directorySeparatorChar)
                                    {
                                        return Path.Name.Substring(0, (int)(end - pathFixed) + 1);
                                    }
                                }
                            }
                        }
                        return null;
                    }
                }
                /// <summary>
                /// 列表名称集合
                /// </summary>
                public serverListName[] ListNames;
                /// <summary>
                /// 是否存在更新
                /// </summary>
                public int IsChange;
                /// <summary>
                /// 获取列表名称集合访问锁
                /// </summary>
                private int getListNameLock;
                /// <summary>
                /// 当前列表名称
                /// </summary>
                public serverListName SelectListName;
                /// <summary>
                /// 当前列表名称集合
                /// </summary>
                public HashSet<hashString> CheckListNames = hashSet.CreateHashString();
                /// <summary>
                /// 获取列表名称集合
                /// </summary>
                public void GetListName()
                {
                    if (Interlocked.Increment(ref getListNameLock) == 1) getListName();
                }
                /// <summary>
                /// 获取列表名称集合
                /// </summary>
                private void getListName()
                {
                    try
                    {
                        IsChange = 0;
                        ServerPermission.View.Client.list(ServerPermission.PathPermission.Path + Path.LowerName, onGetListName);
                        return;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    getListNameError(true);
                }
                /// <summary>
                /// 获取列表名称集合
                /// </summary>
                /// <param name="listNames">列表名称集合</param>
                private void onGetListName(fastCSharp.net.returnValue<server.listName[]> listNames)
                {
                    if (listNames.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        if (listNames.Value == null)
                        {
                            getListNameError(false);
                            tryParentPath();
                        }
                        else
                        {
                            if (IsChange == 0)
                            {
                                OnGetListName(listNames.Value);
                                getListNameLock = 0;
                                ServerPermission.View.Form.context.Post(ServerPermission.View.Form.setServerList, this);
                            }
                            else getListName();
                        }
                    }
                    else getListNameError(true);
                }
                /// <summary>
                /// 获取列表名称集合失败
                /// </summary>
                /// <param name="isClose">是否关闭客户端</param>
                private void getListNameError(bool isClose)
                {
                    getListNameLock = 0;
                    user user = user.Table.Cache.Get(ServerPermission.View.UserId);
                    ServerPermission.View.Form.context.Post(ServerPermission.View.Form.addError, "错误：" + (user == null ? ServerPermission.View.UserId.toString() : user.ToString()) + @" 获取列表名称失败
" + ServerPermission.PathPermission.Path + Path.Name);
                    if (isClose) ServerPermission.View.CreateClient(false);
                }
                /// <summary>
                /// 尝试重新获取父级目录
                /// </summary>
                private void tryParentPath()
                {
                    if (ServerPermission.CurrentPath.LowerName == Path.LowerName)
                    {
                        string parentPath = ParentPath;
                        if (parentPath != null)
                        {
                            ServerPermission.CurrentPath.Name = parentPath;
                            ServerPermission.View.Form.context.Post(ServerPermission.View.Form.setServerPermission, ServerPermission);
                        }
                    }
                }
                /// <summary>
                /// 获取列表名称集合
                /// </summary>
                /// <param name="listNames">列表名称集合</param>
                public void OnGetListName(server.listName[] listNames)
                {
                    serverListName serverListName;
                    Dictionary<hashString, serverListName> nameHash = ListNames.getDictionary(value => (hashString)value.ListName.LowerName);
                    list<serverListName> nameList = new list<serverListName>(listNames.Length);
                    foreach (server.listName value in listNames)
                    {
                        if (nameHash != null && nameHash.TryGetValue(value.LowerName, out serverListName))
                        {
                            serverListName.ListName = value;
                            nameList.UnsafeAdd(serverListName);
                        }
                        else nameList.UnsafeAdd(new serverListName { ServerPath = this, ListName = value });
                    }
                    ListNames = nameList.UnsafeArray ?? nullValue<serverListName>.Array;
                }
                /// <summary>
                /// 删除服务器端列表名称
                /// </summary>
                /// <param name="listNames">列表名称集合</param>
                public void Delete(int backupIdentity, server.listName[] listNames)
                {
                    try
                    {
                        ServerPermission.View.Client.delete(ServerPermission.PathPermission.Path + Path.LowerName, listNames, backupIdentity, onDelete);
                        return;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                    deleteError(true);
                }
                /// <summary>
                /// 删除列表名称集合
                /// </summary>
                /// <param name="listNames">列表名称集合</param>
                private void onDelete(fastCSharp.net.returnValue<server.listName[]> listNames)
                {
                    if (listNames.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        if (listNames.Value == null)
                        {
                            deleteError(false);
                            tryParentPath();
                        }
                        else
                        {
                            OnGetListName(listNames.Value);
                            ServerPermission.View.Form.context.Post(ServerPermission.View.Form.setServerList, this);
                        }
                    }
                    else deleteError(true);
                }
                /// <summary>
                /// 删除列表名称集合失败
                /// </summary>
                /// <param name="isClose">是否关闭客户端</param>
                private void deleteError(bool isClose)
                {
                    user user = user.Table.Cache.Get(ServerPermission.View.UserId);
                    ServerPermission.View.Form.context.Post(ServerPermission.View.Form.addError, "错误：" + (user == null ? ServerPermission.View.UserId.toString() : user.ToString()) + @" 删除列表名称失败
" + ServerPermission.PathPermission.Path + Path.Name);
                    if (isClose) ServerPermission.View.CreateClient(false);
                }
                /// <summary>
                /// 上传文件
                /// </summary>
                /// <param name="listName">本地列表名称</param>
                /// <param name="扩展名过滤">extensionFilterHash</param>
                /// <param name="isTimeVersion">是否匹配时间版本</param>
                public void Upload(int backupIdentity, localListName listName, HashSet<hashString> extensionFilterHash, bool isTimeVersion)
                {
                    uploadInfo uploadInfo = new uploadInfo { BackupIdentity = backupIdentity, ServerPath = this, ExtensionFilter = extensionFilterHash, ListName = listName, IsTimeVersion = isTimeVersion };
                    if (listName.ListName.IsFile) ServerPermission.View.Form.uploadQueue.File(uploadInfo);
                    else ServerPermission.View.Form.uploadQueue.Directory(uploadInfo);
                }
                /// <summary>
                /// 批量上传文件并且添加自动匹配路径
                /// </summary>
                /// <param name="listNames">本地列表名称集合</param>
                /// <param name="extensionFilterHash">扩展名过滤</param>
                /// <param name="extensionFilter">扩展名过滤</param>
                /// <param name="isTimeVersion">是否匹配时间版本</param>
                public void Upload(int backupIdentity, localListName[] listNames, HashSet<hashString> extensionFilterHash, string extensionFilter, bool isTimeVersion)
                {
                    lowerName localPath = default(lowerName);
                    foreach (localListName listName in listNames)
                    {
                        localPath = listName.Path;
                        Upload(backupIdentity, listName, extensionFilterHash, isTimeVersion);
                    }
                    if (localPath.Name != null && extensionFilter != null) autoPath.Set(ServerPermission.View.UserId, ServerPermission.PathPermission.Path + Path.LowerName, localPath, extensionFilter);
                }
                /// <summary>
                /// 下载文件
                /// </summary>
                /// <param name="ListName">服务器端列表名称</param>
                /// <param name="localPath">本地路径</param>
                /// <param name="extensionFilterHash">扩展名过滤</param>
                /// <param name="extensionFilter">扩展名过滤</param>
                /// <param name="isTimeVersion">是否匹配时间版本</param>
                public void Download(serverListName listName, lowerName localPath, HashSet<hashString> extensionFilterHash, string extensionFilter, bool isTimeVersion)
                {
                    downloadInfo downloadInfo = new downloadInfo { ServerPath = this, ExtensionFilter = extensionFilterHash, ListName = listName, LocalPath = localPath, IsTimeVersion = isTimeVersion };
                    if (listName.ListName.IsFile) ServerPermission.View.Form.downloadQueue.File(downloadInfo);
                    else ServerPermission.View.Form.downloadQueue.Directory(downloadInfo);
                    if (extensionFilter != null) autoPath.Set(ServerPermission.View.UserId, ServerPermission.PathPermission.Path + Path.LowerName, localPath, extensionFilter);
                }
                /// <summary>
                /// 下载文件
                /// </summary>
                /// <param name="ListNames">服务器端列表名称集合</param>
                /// <param name="localPath">本地路径</param>
                /// <param name="extensionFilterHash">扩展名过滤</param>
                /// <param name="extensionFilter">扩展名过滤</param>
                /// <param name="isFilter">是否扩展名过滤</param>
                /// <param name="isTimeVersion">是否匹配时间版本</param>
                public void Download(serverListName[] listNames, lowerName localPath, HashSet<hashString> extensionFilterHash, string extensionFilter, bool isFilter, bool isTimeVersion)
                {
                    foreach (serverListName listName in listNames.getSort(value => value.ListName.Length))
                    {
                        if (listName.ListName.IsFile)
                        {
                            if (!isFilter || extensionFilterHash == null || extensionFilterHash.Contains(listName.ExtensionName))
                            {
                                Download(listName, localPath, null, null, isTimeVersion);
                            }
                        }
                        else Download(listName, localPath, extensionFilterHash, null, isTimeVersion);
                    }
                    if (extensionFilter != null) autoPath.Set(ServerPermission.View.UserId, ServerPermission.PathPermission.Path + Path.LowerName, localPath, extensionFilter);
                }
            }
            /// <summary>
            /// 服务器端权限路径
            /// </summary>
            public sealed class serverPermission
            {
                /// <summary>
                /// 用户视图数据
                /// </summary>
                public userView View;
                /// <summary>
                /// 服务器端路径权限
                /// </summary>
                public server.pathPermission PathPermission;
                /// <summary>
                /// 服务器端路径集合
                /// </summary>
                private Dictionary<hashString, serverPath> paths = dictionary.CreateHashString<serverPath>();
                /// <summary>
                /// 当前路径
                /// </summary>
                public lowerName CurrentPath;
                /// <summary>
                /// 当前服务器端路径
                /// </summary>
                public serverPath CurrentServerPath
                {
                    get
                    {
                        if (CurrentPath.Name == null) CurrentPath.Name = string.Empty;
                        return GetServerPath(CurrentPath);
                    }
                }
                /// <summary>
                /// 服务器端路径访问锁
                /// </summary>
                private readonly object serverPathLock = new object();
                /// <summary>
                /// 获取服务器端路径
                /// </summary>
                public serverPath GetServerPath(lowerName path)
                {
                    serverPath serverPath;
                    hashString pathName = path.LowerName;
                    Monitor.Enter(serverPathLock);
                    try
                    {
                        if (!paths.TryGetValue(pathName, out serverPath))
                        {
                            paths.Add(pathName, serverPath = new serverPath { ServerPermission = this, Path = path });
                        }
                    }
                    finally { Monitor.Exit(serverPathLock); }
                    return serverPath;
                }
                /// <summary>
                /// 历史路径
                /// </summary>
                public historyPath HistoryPath = new historyPath();
                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    return PathPermission.ToString();
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public form Form;
            /// <summary>
            /// 新用户
            /// </summary>
            public user NewUser;
            /// <summary>
            /// 用户标识
            /// </summary>
            public int UserId;
            /// <summary>
            /// 文件传输客户端
            /// </summary>
            public server.tcpClient Client;
            /// <summary>
            /// 是否已经释放资源
            /// </summary>
            private int isDisposed;
            /// <summary>
            /// 服务器端权限路径
            /// </summary>
            public serverPermission[] ServerPermissions;
            /// <summary>
            /// 当前服务器端权限路径
            /// </summary>
            public serverPermission SelectServerPermission;
            /// <summary>
            /// 获取服务器端权限路径访问锁
            /// </summary>
            private int getServerPermissionLock;
            /// <summary>
            /// 获取服务器端权限路径
            /// </summary>
            public void GetServerPermission()
            {
                if (Interlocked.Increment(ref getServerPermissionLock) == 1) getServerPermission();
            }
            /// <summary>
            /// 获取服务器端权限路径
            /// </summary>
            private void getServerPermission()
            {
                try
                {
                    Client.getPermissions(onGetServerPermission);
                    return;
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                getServerPermissionError();
            }
            /// <summary>
            /// 获取服务器端权限路径
            /// </summary>
            /// <param name="serverPaths">服务器端权限路径</param>
            private void onGetServerPermission(fastCSharp.net.returnValue<server.pathPermission[]> serverPaths)
            {
                if (serverPaths.Type == fastCSharp.net.returnValue.type.Success && serverPaths.Value != null)
                {
                    serverPermission serverPath;
                    Dictionary<hashString, serverPermission> pathHash = ServerPermissions.getDictionary(value => (hashString)value.PathPermission.Path);
                    list<serverPermission> serverPathList = new list<serverPermission>(serverPaths.Value.Length);
                    foreach (server.pathPermission value in serverPaths.Value)
                    {
                        if (pathHash != null && pathHash.TryGetValue(value.Path, out serverPath))
                        {
                            serverPath.PathPermission = value;
                            serverPathList.UnsafeAdd(serverPath);
                        }
                        else serverPathList.UnsafeAdd(new serverPermission { View = this, PathPermission = value });
                    }
                    ServerPermissions = serverPathList.UnsafeArray ?? nullValue<serverPermission>.Array;
                    getServerPermissionLock = 0;
                    Form.context.Post(Form.setServerPermissions, this);
                }
                else getServerPermissionError();
            }
            /// <summary>
            /// 获取服务器端权限路径失败
            /// </summary>
            private void getServerPermissionError()
            {
                getServerPermissionLock = 0;
                user user = user.Table.Cache.Get(UserId);
                Form.context.Post(Form.addError, "错误：" + (user == null ? UserId.toString() : user.ToString()) + " 获取服务器端路径失败");
                CreateClient(false);
            }
            /// <summary>
            /// 创建客户端连接
            /// </summary>
            /// <param name="isCount">是否计数</param>
            public void CreateClient(bool isCount)
            {
                if (isDisposed == 0)
                {
                    user user = UserId >= 0 ? user.Table.Cache.Get(UserId) : NewUser;
                    if (user != null)
                    {
                        server.tcpClient client = null;
                        try
                        {
                            fastCSharp.code.cSharp.tcpServer attribute = fastCSharp.code.cSharp.tcpServer.GetConfig(typeof(fastCSharp.demo.fileTransferServer.server));
                            attribute.Host = user.Host;
                            attribute.Port = user.Port;
                            client = new server.tcpClient(attribute, this);
                            if (client.TcpCommandClient.StreamSocket != null)
                            {
                                Interlocked.Exchange(ref Client, client);
                                client = null;
                                Form.context.Post(Form.addMessage, "服务器连接成功 " + user.ToString());
                                if (isCount) user.IncUserCount();
                                else if (UserId < 0)
                                {
                                    user newUser = fastCSharp.emit.memberCopyer<user>.MemberwiseClone(user);
                                    newUser.Id = 0;
                                    if ((newUser = user.Table.Insert(newUser, false)) == null)
                                    {
                                        Form.context.Post(Form.addError, "错误：用户添加失败 " + user.ToString());
                                        NewUser = null;
                                    }
                                    else UserId = newUser.Id;
                                }
                            }
                            else Form.context.Post(Form.addError, "错误：服务器连接或验证失败 " + user.ToString());
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            Form.context.Post(Form.addError, "错误：服务器连接失败 " + user.ToString());
                        }
                        finally
                        {
                            pub.Dispose(ref client);
                            Form.removeCurrent(user, UserId);
                            if (isDisposed != 0) Dispose();
                        }
                    }
                }
            }
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            /// <returns>是否通过验证</returns>
            public bool Verify(server.tcpClient client)
            {
                user user = UserId >= 0 ? user.Table.Cache.Get(UserId) : NewUser;
                if (user != null)
                {
                    DateTime verifyTime = newVerifyTime;
                    return client.login(user.Name, server.Md5Password(user.Password, verifyTime), verifyTime).Value;
                }
                return false;
            }
            /// <summary>
            /// 关闭客户端
            /// </summary>
            public void Close()
            {
                pub.Dispose(ref Client);
            }
            /// <summary>
            /// 关闭客户端
            /// </summary>
            public void Dispose()
            {
                isDisposed = 1;
                Close();
            }
            /// <summary>
            /// 验证时间
            /// </summary>
            private static DateTime verifyTime;
            /// <summary>
            /// 验证时间访问锁
            /// </summary>
            private static int verifyTimeLock;
            /// <summary>
            /// 验证时间
            /// </summary>
            private static DateTime newVerifyTime
            {
                get
                {
                    DateTime time = date.NowSecond;
                    interlocked.CompareSetYield(ref verifyTimeLock);
                    if (time == verifyTime) time = time.AddMilliseconds(1);
                    verifyTime = time;
                    verifyTimeLock = 0;
                    return time;
                }
            }
        }
        /// <summary>
        /// 本地列表名称
        /// </summary>
        private sealed class localListName
        {
            /// <summary>
            /// 路径名称
            /// </summary>
            public lowerName Path;
            /// <summary>
            /// 列表名称
            /// </summary>
            public server.listName ListName;
            /// <summary>
            /// 本地列表名称
            /// </summary>
            /// <param name="path">路径名称</param>
            /// <param name="file">文件信息</param>
            public localListName(lowerName path, FileInfo file)
            {
                Path = path;
                Set(file);
            }
            /// <summary>
            /// 本地列表名称
            /// </summary>
            /// <param name="path">路径名称</param>
            /// <param name="directory">目录信息</param>
            public localListName(lowerName path, DirectoryInfo directory)
            {
                Path = path;
                Set(directory);
            }
            /// <summary>
            /// 设置文件信息
            /// </summary>
            /// <param name="file">文件信息</param>
            public void Set(FileInfo file)
            {
                ListName.Set(file.Name, file.LastWriteTimeUtc, file.Length);
            }
            /// <summary>
            /// 设置目录信息
            /// </summary>
            /// <param name="directory">目录信息</param>
            public void Set(DirectoryInfo directory)
            {
                ListName.Set(directory.Name, directory.LastWriteTimeUtc, long.MinValue);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return toString(ListName);
            }
        }
        /// <summary>
        /// 文件传输信息
        /// </summary>
        private abstract class transferInfo
        {
            /// <summary>
            /// 服务器端路径
            /// </summary>
            public userView.serverPath ServerPath;
            /// <summary>
            /// 扩展名过滤
            /// </summary>
            public HashSet<hashString> ExtensionFilter;
            /// <summary>
            /// 
            /// </summary>
            protected form form
            {
                get { return ServerPath.ServerPermission.View.Form; }
            }
            /// <summary>
            /// 文件流
            /// </summary>
            protected FileStream fileStream;
            /// <summary>
            /// 是否匹配时间版本
            /// </summary>
            public bool IsTimeVersion;
            /// <summary>
            /// 操作状态
            /// </summary>
            public server.fileState FileState { get; protected set; }
            /// <summary>
            /// 开始文件传输
            /// </summary>
            public abstract void File();
            /// <summary>
            /// 开始目录传输
            /// </summary>
            public abstract void Directory();
        }
        /// <summary>
        /// 文件上传信息
        /// </summary>
        private sealed class uploadInfo : transferInfo
        {
            /// <summary>
            /// 备份路径编号
            /// </summary>
            public int BackupIdentity;
            /// <summary>
            /// 列表名称
            /// </summary>
            public localListName ListName;
            /// <summary>
            /// 开始文件传输
            /// </summary>
            public override void File()
            {
                string fileName = ListName.Path.Name + ListName.ListName.Name;
                int isNext = 0, isCall = 0;
                try
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (fileInfo.Exists)
                    {
                        if (fileInfo.Length <= fastCSharp.config.tcpCommand.Default.BigBufferSize)
                        {
                            isCall = 1;
                            form.context.Post(form.addMessage, "开始上传文件 " + fileName);
                            ServerPath.ServerPermission.View.Client.upload(new server.listName { Name = ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name, LastWriteTime = fileInfo.LastWriteTimeUtc }, System.IO.File.ReadAllBytes(fileName), BackupIdentity, IsTimeVersion, onFile);
                            isNext = 1;
                        }
                        else
                        {
                            fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                            isCall = 1;
                            form.context.Post(form.addMessage, "开始上传文件 " + fileName);
                            ServerPath.ServerPermission.View.Client.upload(new server.listName { Name = ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name, LastWriteTime = fileInfo.LastWriteTimeUtc, Length = fileInfo.Length }, fileStream, BackupIdentity, IsTimeVersion, onFile);
                            isNext = 1;
                        }
                    }
                    else form.context.Post(form.addMessage, "没有找到本地文件 " + fileName);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                    form.context.Post(form.addError, (isCall == 0 ? "错误：本地文件访问失败 " : "网络错误：文件上传失败 ") + fileName);
                }
                finally
                {
                    if (isNext == 0)
                    {
                        pub.Dispose(ref fileStream);
                        if (form.uploadQueue.Next()) form.context.Post(form.addMessage, "上传任务已经结束");
                    }
                }
            }
            /// <summary>
            /// 文件操作结束
            /// </summary>
            /// <param name="fileState">文件操作状态</param>
            private void onFile(fastCSharp.net.returnValue<server.fileState> fileState)
            {
                string fileName = ListName.Path.Name + ListName.ListName.Name;
                try
                {
                    if (fileState.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        FileState = fileState.Value;
                        ServerPath.IsChange = 1;
                        form.context.Post(form.onUploadFile, this);
                    }
                    else form.context.Post(form.addError, "网络错误：文件上传失败 " + fileName);
                }
                finally
                {
                    pub.Dispose(ref fileStream);
                    if (form.uploadQueue.Next()) form.context.Post(form.addMessage, "上传任务已经结束");
                }
            }
            /// <summary>
            /// 开始目录传输
            /// </summary>
            public override void Directory()
            {
                string directoryName = ListName.Path.Name + ListName.ListName.Name;
                int isNext = 0;
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                    if (directoryInfo.Exists)
                    {
                        ServerPath.ServerPermission.View.Client.createDirectory(ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name, onDirectory);
                        isNext = 1;
                    }
                    else form.context.Post(form.addMessage, "没有找到本地目录 " + directoryName);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                    form.context.Post(form.addError, "网络错误：目录访问失败 " + ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name);
                }
                finally
                {
                    if (isNext == 0 && form.uploadQueue.Next()) form.context.Post(form.addMessage, "上传任务已经结束");
                }
            }
            /// <summary>
            /// 目录操作结束
            /// </summary>
            /// <param name="fileState">目录操作状态</param>
            private void onDirectory(fastCSharp.net.returnValue<server.fileState> fileState)
            {
                try
                {
                    if (fileState.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        FileState = fileState.Value;
                        if (FileState == server.fileState.Success || FileState == server.fileState.FileNotFound)
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(ListName.Path.Name + ListName.ListName.Name);
                            if (directoryInfo.Exists)
                            {
                                string directoryName = directoryInfo.fullName();
                                userView.serverPath serverPath = null;
                                foreach (FileInfo file in directoryInfo.GetFiles().getSort(value => value.Length))
                                {
                                    if (ExtensionFilter == null || isExtensionFilter(ExtensionFilter, file))
                                    {
                                        if (serverPath == null) serverPath = ServerPath.ServerPermission.GetServerPath(new lowerName { Name = ServerPath.Path.Name + ListName.ListName.Name + directory.DirectorySeparator });
                                        serverPath.Upload(BackupIdentity, new localListName(new lowerName { Name = directoryName }, file), null, IsTimeVersion);
                                    }
                                }
                                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                                {
                                    if (serverPath == null) serverPath = ServerPath.ServerPermission.GetServerPath(new lowerName { Name = ServerPath.Path.Name + ListName.ListName.Name + fastCSharp.directory.DirectorySeparator });
                                    serverPath.Upload(BackupIdentity, new localListName(new lowerName { Name = directoryName }, directory), ExtensionFilter, IsTimeVersion);
                                }
                            }
                            if (FileState == server.fileState.FileNotFound)
                            {
                                ServerPath.IsChange = 1;
                                if (ServerPath == form.currentServerPath) form.context.Post(form.onUploadDirectory, this);
                            }
                        }
                        else form.context.Post(form.addError, "错误：目录访问失败[" + fileState.Value.ToString() + "] " + ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name);
                    }
                    else form.context.Post(form.addError, "网络错误：目录访问失败 " + ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name);
                }
                finally
                {
                    if (form.uploadQueue.Next()) form.context.Post(form.addMessage, "上传任务已经结束");
                }
            }
        }
        /// <summary>
        /// 文件下载信息
        /// </summary>
        private sealed class downloadInfo : transferInfo
        {
            /// <summary>
            /// 服务器端列表名称
            /// </summary>
            public userView.serverListName ListName;
            /// <summary>
            /// 本地路径
            /// </summary>
            public lowerName LocalPath;
            /// <summary>
            /// 本地文件起始长度
            /// </summary>
            private long localLength;
            /// <summary>
            /// 开始文件传输
            /// </summary>
            public override void File()
            {
                file(false);
            }
            /// <summary>
            /// 开始文件传输
            /// </summary>
            /// <param name="isChange">服务器端文件信息是否更新</param>
            private void file(bool isChange)
            {
                string fileName = LocalPath.Name + ListName.ListName.Name;
                int isNext = 0, isCall = 0, timeVersion = 0;
                try
                {
                    localLength = 0;
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (fileInfo.Exists)
                    {
                        if (IsTimeVersion && fileInfo.LastWriteTimeUtc > ListName.ListName.LastWriteTime) timeVersion = 1;
                        else if (fileInfo.LastWriteTimeUtc == ListName.ListName.LastWriteTime && fileInfo.Length <= ListName.ListName.Length)
                        {
                            fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.None);
                            fileStream.Seek(localLength = fileInfo.Length, SeekOrigin.Begin);
                        }
                        else fileInfo.Delete();
                    }
                    if (timeVersion == 0)
                    {
                        if (fileStream == null)
                        {
                            fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        }
                        if (ListName.ListName.Length != 0)
                        {
                            if (!isChange) form.context.Post(form.addMessage, "开始下载文件 " + fileName);
                            isCall = 1;
                            ServerPath.ServerPermission.View.Client.download(new server.listName { Name = ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name, LastWriteTime = ListName.ListName.LastWriteTime, Length = ListName.ListName.Length }, fileStream, onFile);
                            isNext = 1;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                    form.context.Post(form.addError, (isCall == 0 ? "错误：本地文件访问失败 " : "网络错误：文件下载失败 ") + fileName);
                }
                finally
                {
                    if (isNext == 0)
                    {
                        pub.Dispose(ref fileStream);
                        if (form.downloadQueue.Next()) form.context.Post(form.addMessage, "下载任务已经结束");
                    }
                }
            }
            /// <summary>
            /// 文件操作结束
            /// </summary>
            /// <param name="listName">服务器端列表名称</param>
            private void onFile(fastCSharp.net.returnValue<server.listName> listName)
            {
                string fileName = LocalPath.Name + ListName.ListName.Name;
                int isNext = 0;
                try
                {
                    pub.Dispose(ref fileStream);
                    if (listName.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        if (listName.Value.Name == null)
                        {
                            FileState = (server.fileState)(byte)listName.Value.Length;
                            form.context.Post(form.onDownloadFile, this);
                        }
                        else if (listName.Value.Name.Length == 0)
                        {
                            ListName.ListName.Length = listName.Value.Length;
                            ListName.ListName.LastWriteTime = listName.Value.LastWriteTime;
                            isNext = 1;
                            file(true);
                        }
                        else
                        {
                            ListName.ListName.Length = listName.Value.Length;
                            new FileInfo(fileName).LastWriteTimeUtc = ListName.ListName.LastWriteTime = listName.Value.LastWriteTime;
                            FileState = server.fileState.Success;
                            form.context.Post(form.onDownloadFile, this);
                        }
                    }
                    else
                    {
                        long fileLength = 0;
                        FileInfo fileInfo = new FileInfo(fileName);
                        if (fileInfo.Exists && (fileLength = fileInfo.Length) != localLength)
                        {
                            fileInfo.LastWriteTimeUtc = ListName.ListName.LastWriteTime;
                        }
                        form.context.Post(form.addError, "网络错误：文件下载失败 " + (fileLength == 0 ? null : ("[" + fileLength.toString() + "/" + ListName.ListName.Length.toString() + "] ")) + fileName);
                    }
                }
                finally
                {
                    if (isNext == 0 && form.downloadQueue.Next()) form.context.Post(form.addMessage, "下载任务已经结束");
                }
            }
            /// <summary>
            /// 开始目录传输
            /// </summary>
            public override void Directory()
            {
                string directoryName = LocalPath.Name + ListName.ListName.Name;
                int isNext = 0, isCall = 0;
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                        lowerName currentLocalPath = form.currentLocalPath;
                        if (currentLocalPath.LowerName == LocalPath.LowerName) form.context.Post(form.onCreateDirectory, directoryName);
                    }
                    userView.serverPath serverPath = ServerPath.ServerPermission.GetServerPath(new lowerName { Name = ServerPath.Path.Name + ListName.ListName.Name + directory.DirectorySeparator });
                    if (serverPath.ListNames == null || serverPath.IsChange != 0)
                    {
                        isCall = 1;
                        serverPath.ServerPermission.View.Client.list(serverPath.ServerPermission.PathPermission.Path + serverPath.Path.LowerName, onGetListName);
                        isNext = 1;
                    }
                    else serverPath.Download(serverPath.ListNames.copy(), new lowerName { Name = directoryName + directory.DirectorySeparator }, ExtensionFilter, null, true, IsTimeVersion);
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                    form.context.Post(form.addError, isCall == 0 ? ("错误：目录访问失败 " + directoryName) : ("网络错误：目录访问失败 " + ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name));
                }
                finally
                {
                    if (isNext == 0 && form.downloadQueue.Next()) form.context.Post(form.addMessage, "下载任务已经结束");
                }
            }
            /// <summary>
            /// 获取列表名称集合
            /// </summary>
            /// <param name="listNames">列表名称集合</param>
            private void onGetListName(fastCSharp.net.returnValue<server.listName[]> listNames)
            {
                string directoryName = LocalPath.Name + ListName.ListName.Name;
                try
                {
                    if (listNames.Type == fastCSharp.net.returnValue.type.Success)
                    {
                        if (listNames.Value == null)
                        {
                            form.context.Post(form.addError, "错误：目录访问失败 " + ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name);
                        }
                        else
                        {
                            userView.serverPath serverPath = ServerPath.ServerPermission.GetServerPath(new lowerName { Name = ServerPath.Path.Name + ListName.ListName.Name + directory.DirectorySeparator });
                            serverPath.OnGetListName(listNames.Value);
                            serverPath.Download(serverPath.ListNames.copy(), new lowerName { Name = directoryName + directory.DirectorySeparator }, ExtensionFilter, null, true, IsTimeVersion);
                        }
                    }
                    else form.context.Post(form.addError, "网络错误：目录访问失败 " + ServerPath.ServerPermission.PathPermission.Path + ServerPath.Path.Name + ListName.ListName.Name);
                }
                finally
                {
                    if (form.downloadQueue.Next()) form.context.Post(form.addMessage, "下载任务已经结束");
                }
            }
        }
        /// <summary>
        /// 传输队列
        /// </summary>
        /// <typeparam name="transferType"></typeparam>
        private sealed class transferQueue<transferType> where transferType : transferInfo
        {
            /// <summary>
            /// 最大线程数量
            /// </summary>
            private int maxThreadCount;
            /// <summary>
            /// 当前线程数量
            /// </summary>
            private int currentThreadCount;
            /// <summary>
            /// 队列操作访问锁
            /// </summary>
            private readonly object queueLock = new object();
            /// <summary>
            /// 文件队列
            /// </summary>
            private list<transferType> fileQueue = new list<transferType>();
            /// <summary>
            /// 目录队列
            /// </summary>
            private list<transferType> directoryQueue = new list<transferType>();
            /// <summary>
            /// 文件传输
            /// </summary>
            /// <param name="transferInfo">文件传输信息</param>
            public void File(transferType transferInfo)
            {
                Monitor.Enter(queueLock);
                try
                {
                    if (currentThreadCount < maxThreadCount) ++currentThreadCount;
                    else
                    {
                        fileQueue.Add(transferInfo);
                        transferInfo = null;
                    }
                }
                finally { Monitor.Exit(queueLock); }
                if (transferInfo != null) fastCSharp.threading.threadPool.TinyPool.Start(transferInfo.File);
            }
            /// <summary>
            /// 目录传输
            /// </summary>
            /// <param name="transferInfo">目录传输信息</param>
            public void Directory(transferType transferInfo)
            {
                Monitor.Enter(queueLock);
                try
                {
                    if (fileQueue.Count == 0 && currentThreadCount < maxThreadCount) ++currentThreadCount;
                    else
                    {
                        directoryQueue.Add(transferInfo);
                        transferInfo = null;
                    }
                }
                finally { Monitor.Exit(queueLock); }
                if (transferInfo != null) fastCSharp.threading.threadPool.TinyPool.Start(transferInfo.Directory);
            }
            /// <summary>
            /// 处理下一个传输
            /// </summary>
            /// <returns>是否完成所有任何</returns>
            public bool Next()
            {
                transferType file = null, directory = null;
                int threadCount = int.MaxValue;
                Monitor.Enter(queueLock);
                if (currentThreadCount <= maxThreadCount)
                {
                    if (fileQueue.Count == 0)
                    {
                        if (directoryQueue.Count == 0) threadCount = --currentThreadCount;
                        else directory = directoryQueue.Pop();
                    }
                    else file = fileQueue.Pop();
                }
                else --currentThreadCount;
                Monitor.Exit(queueLock);
                if (file == null)
                {
                    if (directory == null)
                    {
                        if (threadCount == 0) return true;
                    }
                    else directory.Directory();
                }
                else file.File();
                return false;
            }
            /// <summary>
            /// 设置最大线程数量
            /// </summary>
            /// <param name="maxThreadCount">最大线程数量</param>
            public void SetMaxThreadCount(int maxThreadCount)
            {
                Monitor.Enter(queueLock);
                this.maxThreadCount = maxThreadCount;
                Monitor.Exit(queueLock);
                do
                {
                    transferType file = null, directory = null;
                    Monitor.Enter(queueLock);
                    if (currentThreadCount < this.maxThreadCount)
                    {
                        if (fileQueue.Count == 0)
                        {
                            if (directoryQueue.Count != 0)
                            {
                                ++currentThreadCount;
                                directory = directoryQueue.Pop();
                            }
                        }
                        else
                        {
                            ++currentThreadCount;
                            file = fileQueue.Pop();
                        }
                    }
                    Monitor.Exit(queueLock);
                    if (file == null)
                    {
                        if (directory == null) break;
                        else fastCSharp.threading.threadPool.TinyPool.Start(directory.Directory);
                    }
                    else fastCSharp.threading.threadPool.TinyPool.Start(file.File);
                }
                while (true);
            }
            /// <summary>
            /// 是否存在未完成任务
            /// </summary>
            /// <param name="userView">用户数据视图</param>
            /// <returns>是否存在未完成任务</returns>
            public bool IsUserView(userView userView)
            {
                int isTransfer = 0;
                Monitor.Enter(queueLock);
                int count = fileQueue.Count;
                if (count != 0)
                {
                    foreach (transferType transferInfo in fileQueue.UnsafeArray)
                    {
                        if (transferInfo.ServerPath.ServerPermission.View == userView)
                        {
                            isTransfer = 1;
                            break;
                        }
                    }
                }
                if (isTransfer == 0 && (count = directoryQueue.Count) != 0)
                {
                    foreach (transferType transferInfo in directoryQueue.UnsafeArray)
                    {
                        if (transferInfo.ServerPath.ServerPermission.View == userView)
                        {
                            isTransfer = 1;
                            break;
                        }
                    }
                }
                Monitor.Exit(queueLock);
                return isTransfer != 0;
            }
        }
        /// <summary>
        /// 备份路径编号获取
        /// </summary>
        private sealed class uploadIdentityGetter
        {
            /// <summary>
            /// 服务器端路径
            /// </summary>
            public userView.serverPath ServerPath;
            /// <summary>
            /// 本地列表名称集合
            /// </summary>
            public localListName[] ListNames;
            /// <summary>
            /// 扩展名过滤
            /// </summary>
            public HashSet<hashString> ExtensionFilterHash;
            /// <summary>
            /// 扩展名过滤
            /// </summary>
            public string ExtensionFilter;
            /// <summary>
            /// 是否匹配时间版本
            /// </summary>
            public bool IsTimeVersion;
            /// <summary>
            /// 备份路径编号获取
            /// </summary>
            /// <param name="value"></param>
            public void OnGetIdentity(fastCSharp.net.returnValue<int> value)
            {
                if (value.Type == fastCSharp.net.returnValue.type.Success) ServerPath.Upload(value.Value, ListNames, ExtensionFilterHash, ExtensionFilter, IsTimeVersion);
                else
                {
                    user user = user.Table.Cache.Get(ServerPath.ServerPermission.View.UserId);
                    ServerPath.ServerPermission.View.Form.context.Post(ServerPath.ServerPermission.View.Form.addError, "错误：" + (user == null ? ServerPath.ServerPermission.View.UserId.toString() : user.ToString()) + " 获取备份路径编号失败");
                }
            }
        }
        /// <summary>
        /// 备份路径编号获取
        /// </summary>
        private sealed class deleteIdentityGetter
        {
            /// <summary>
            /// 服务器端路径
            /// </summary>
            public userView.serverPath ServerPath;
            /// <summary>
            /// 本地列表名称集合
            /// </summary>
            public server.listName[] ListNames;
            /// <summary>
            /// 备份路径编号获取
            /// </summary>
            /// <param name="value"></param>
            public void OnGetIdentity(fastCSharp.net.returnValue<int> value)
            {
                if (value.Type == fastCSharp.net.returnValue.type.Success) ServerPath.Delete(value.Value, ListNames);
                else
                {
                    user user = user.Table.Cache.Get(ServerPath.ServerPermission.View.UserId);
                    ServerPath.ServerPermission.View.Form.context.Post(ServerPath.ServerPermission.View.Form.addError, "错误：" + (user == null ? ServerPath.ServerPermission.View.UserId.toString() : user.ToString()) + " 获取备份路径编号失败");
                }
            }
        }
        /// <summary>
        /// UI线程上下文
        /// </summary>
        private readonly SynchronizationContext context;
        /// <summary>
        /// 用户视图数据集合
        /// </summary>
        private Dictionary<int, userView> userViews = dictionary.CreateInt<userView>();
        /// <summary>
        /// 用户视图数据访问锁
        /// </summary>
        private readonly object userViewLock = new object();
        /// <summary>
        /// 当前选择用户标识
        /// </summary>
        private int currentUserId;
        /// <summary>
        /// 正在连接的客户端集合
        /// </summary>
        private Dictionary<user, userView> currentClients = dictionary.CreateOnly<user, userView>();
        /// <summary>
        /// 正在连接的客户端集合访问锁
        /// </summary>
        private readonly object currentClientLock = new object();
        /// <summary>
        /// 正在连接的客户端标识
        /// </summary>
        private int currentClientIdentity;
        /// <summary>
        /// 本地路径
        /// </summary>
        private lowerName currentLocalPath;
        /// <summary>
        /// 当前服务器端路径
        /// </summary>
        private userView.serverPath currentServerPath;
        /// <summary>
        /// 本地路径历史记录
        /// </summary>
        private historyPath localHistoryPath = new historyPath();
        /// <summary>
        /// 上传队列
        /// </summary>
        private transferQueue<uploadInfo> uploadQueue = new transferQueue<uploadInfo>();
        /// <summary>
        /// 下载队列
        /// </summary>
        private transferQueue<downloadInfo> downloadQueue = new transferQueue<downloadInfo>();
        /// <summary>
        /// 信息编号
        /// </summary>
        private int messageIdentity;
        /// <summary>
        /// 扩展名过滤
        /// </summary>
        private string currentExtensionFilter = string.Empty;
        /// <summary>
        /// 扩展名过滤
        /// </summary>
        private HashSet<hashString> currentExtensionFilterHash;
        /// <summary>
        /// 获取扩展名过滤
        /// </summary>
        private HashSet<hashString> extensionFilterHash
        {
            get
            {
                if (isChangeExtensionFilter)
                {
                    currentExtensionFilter = extensionFilterTextBox.Text.Trim().toLower();
                    currentExtensionFilterHash = currentExtensionFilter.Length == 0 ? null : currentExtensionFilter.split('/').getHash(value => (hashString)value);
                }
                return currentExtensionFilterHash;
            }
        }
        /// <summary>
        /// 扩展名过滤是否更新
        /// </summary>
        private bool isChangeExtensionFilter
        {
            get
            {
                string extensionFilter = extensionFilterTextBox.Text.Trim();
                return extensionFilter.Length != currentExtensionFilter.Length || !extensionFilter.equalCase(currentExtensionFilter, extensionFilter.Length);
            }
        }
        public form()
        {
            InitializeComponent();

            context = SynchronizationContext.Current;

            user.Table.Cache.WaitLoad();
            user.Table.Cache.OnInserted += onUserChange;
            user.Table.Cache.OnDeleted += onUserChange;
            user.Table.Cache.OnUpdated += onUserChange;
            if (user.Table.Cache.Count == 0)
            {
                fastCSharp.code.cSharp.tcpServer attribute = fastCSharp.code.cSharp.tcpServer.GetConfig(typeof(fastCSharp.demo.fileTransferServer.server));
                if (attribute != null)
                {
                    hostTextBox.Text = attribute.Host;
                    portTextBox.Text = ((ushort)attribute.Port).toString();
                }
            }
            else onUserChange((user)null);

            setMaxUploadThreadCount(null, null);
            setMaxDownloadThreadCount(null, null);
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close(object sender, FormClosedEventArgs e)
        {
            Monitor.Enter(userViewLock);
            foreach (userView userView in userViews.Values) userView.Dispose();
            userViews.Clear();
            Monitor.Exit(userViewLock);
            Monitor.Enter(currentClientLock);
            foreach (userView userView in currentClients.Values) userView.Dispose();
            currentClients.Clear();
            Monitor.Exit(currentClientLock);
            user.Table.Dispose();
            autoPath.Table.Dispose();
        }
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="value"></param>
        private void addMessage(object value)
        {
            messageTextBox.AppendText(messageTextBox.Text.Length == 0 ? "[" + (++messageIdentity).toString() + "]" + (string)value : (@"
[" + (++messageIdentity).toString() + "]" + (string)value));
            messageTextBox.ScrollToCaret();
            clearMessageButton.Enabled = true;
        }
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="value"></param>
        private void addError(object value)
        {
            errorTextBox.AppendText(errorTextBox.Text.Length == 0 ? "[" + (++messageIdentity).toString() + "]" + (string)value : (@"
[" + (++messageIdentity).toString() + "]" +  (string)value));
            errorTextBox.ScrollToCaret();
            clearErrorButton.Enabled = true;
        }
        /// <summary>
        /// 清除信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearMessageButton_Click(object sender, EventArgs e)
        {
            messageTextBox.Text = string.Empty;
            clearMessageButton.Enabled = false;
        }
        /// <summary>
        /// 清除错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearErrorButton_Click(object sender, EventArgs e)
        {
            errorTextBox.Text = string.Empty;
            clearErrorButton.Enabled = false;
        }
        /// <summary>
        /// 用户更新事件
        /// </summary>
        /// <param name="value"></param>
        private void onUserChange(object value)
        {
            object selectUser = userComboBox.SelectedItem;
            user[] users = (user[])value;
            userComboBox.Items.Clear();
            userComboBox.Items.AddRange(users);
            bool isSetUser = false;
            if (selectUser != null) selectUser = user.Table.Cache.Get(((user)selectUser).Id);
            if (selectUser == null && users.Length != 0)
            {
                selectUser = users[0];
                isSetUser = true;
            }
            if (selectUser != null)
            {
                userComboBox.SelectedItem = selectUser;
                if (isSetUser) setUser((user)selectUser);
                else setUserButton((user)selectUser);
            }
        }
        /// <summary>
        /// 用户更新事件
        /// </summary>
        /// <param name="value"></param>
        private void onUserChange(user _)
        {
            context.Post(onUserChange, user.Table.Cache.GetSubArray().getSortDesc(value => value.UseCount));
        }
        /// <summary>
        /// 用户更新事件
        /// </summary>
        /// <param name="value"></param>
        private void onUserChange(user _, user __, memberMap memberMap)
        {
            onUserChange(_);
        }
        /// <summary>
        /// 设置用户按钮状态
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private userView setUserButton(user value)
        {
            currentUserId = value.Id;
            hostTextBox.Text = value.Host;
            portTextBox.Text = value.Port.toString();
            userNameTextBox.Text = value.Name;
            passwordTextBox.Text = value.Password;

            userView view;
            Monitor.Enter(userViewLock);
            try
            {
                if (!userViews.TryGetValue(value.Id, out view))
                {
                    Monitor.Enter(currentClientLock);
                    bool isCurrent = currentClients.TryGetValue(value, out view);
                    if (isCurrent) view.UserId = value.Id;
                    Monitor.Exit(currentClientLock);
                    if (!isCurrent) userViews.Add(value.Id, view = new userView { Form = this, UserId = value.Id });
                }
            }
            finally { Monitor.Exit(userViewLock); }
            clientButton.Enabled = (view.Client == null);
            closeButton.Enabled = (view.Client != null);
            return view;
        }
        /// <summary>
        /// 设置当前用户
        /// </summary>
        /// <param name="value">当前用户</param>
        private void setUser(user value)
        {
            userView view = setUserButton(value);
            pathComboBox.Items.Clear();
            pathComboBox.Items.AddRange(autoPath.UserCache.GetCache(currentUserId).getArray(path => path.LocalPath).getFindArray(path => Directory.Exists(path)).sort((left, right) => left.CompareTo(right)));
            parentServerPathButton.Enabled = previousServerPathButton.Enabled = nextServerPathButton.Enabled = false;
            uploadButton.Enabled = downloadButton.Enabled = false;
            clearServerPermissions();
            clearServerList();
            if (view.Client != null)
            {
                if (view.ServerPermissions == null) view.GetServerPermission();
                else setServerPermissions(view);
            }
        }
        /// <summary>
        /// 清除服务器端权限路径
        /// </summary>
        private void clearServerPermissions()
        {
            object selectItem = serverPermissionComboBox.SelectedItem;
            serverPermissionComboBox.Items.Clear();
            if (selectItem != null)
            {
                userView.serverPermission serverPermission = (userView.serverPermission)selectItem;
                serverPermission.View.SelectServerPermission = serverPermission;
            }
        }
        /// <summary>
        /// 设置服务器端权限路径
        /// </summary>
        /// <param name="value">用户视图数据</param>
        private void setServerPermissions(object value)
        {
            userView view = (userView)value;
            if (view.UserId == currentUserId)
            {
                userView.serverPermission[] serverPermissions = view.ServerPermissions;
                clearServerPermissions();
                serverPermissionComboBox.Items.AddRange(serverPermissions);
                if (view.SelectServerPermission != null && serverPermissions.indexOf(view.SelectServerPermission) == -1)
                {
                    view.SelectServerPermission = null;
                }
                if (view.SelectServerPermission == null && serverPermissions.Length != 0) view.SelectServerPermission = serverPermissions[0];
                if (view.SelectServerPermission != null) setServerPermission(serverPermissionComboBox.SelectedItem = view.SelectServerPermission);
            }
        }
        /// <summary>
        /// 获取当前服务器端权限路径
        /// </summary>
        /// <param name="value">服务器端权限路径</param>
        /// <returns>当前服务器端权限路径,null表示失败</returns>
        private userView.serverPermission getCurrentServerPermission(object value)
        {
            userView.serverPermission serverPermission = (userView.serverPermission)value;
            userView view = serverPermission.View;
            if (currentUserId == view.UserId)
            {
                object selectPermission = serverPermissionComboBox.SelectedItem;
                if (selectPermission != null && ((userView.serverPermission)selectPermission).PathPermission.Path == serverPermission.PathPermission.Path)
                {
                    return (userView.serverPermission)selectPermission;
                }
            }
            return null;
        }
        /// <summary>
        /// 设置服务器端权限路径
        /// </summary>
        /// <param name="value">服务器端权限路径</param>
        private void setServerPermission(object value)
        {
            userView.serverPermission serverPermission = getCurrentServerPermission(value);
            if (serverPermission != null) setServerPermission(serverPermission, true);
        }
        /// <summary>
        /// 设置服务器端权限路径
        /// </summary>
        /// <param name="serverPermission">服务器端权限路径</param>
        /// <param name="isSetHistory">是否设置历史路径</param>
        /// <returns>是否成功设置服务器端列表名称</returns>
        private bool setServerPermission(userView.serverPermission serverPermission, bool isSetHistory)
        {
            userView.serverPath serverPath = serverPermission.CurrentServerPath;
            if (currentLocalPath.Name == null || autoCheckBox.Checked)
            {
                autoPath autoPath = autoPath.Table.Cache.Get(new autoPath.primaryKey { ServerPath = serverPermission.PathPermission.Path + serverPath.Path.LowerName, UserId = serverPermission.View.UserId });
                if (autoPath != null)
                {
                    if (autoCheckBox.Checked) extensionFilterTextBox.Text = autoPath.ExtensionFilter;
                    setLocalPath(new lowerName { Name = autoPath.LocalPath }, true, false);
                }
            }
            bool isServerList = false;
            if (serverPath.ListNames == null || serverPath.IsChange != 0)
            {
                refreshServerListNameButton.Enabled = false;
                serverPathTextBox.Text = "? " + serverPath.Path.Name;
                serverPath.GetListName();
            }
            else
            {
                isServerList = true;
                setServerList(serverPath, isSetHistory);
            }
            return isServerList;
        }
        /// <summary>
        /// 获取当前服务器端路径
        /// </summary>
        /// <param name="value">服务器端路径</param>
        /// <returns>当前服务器端路径,null表示失败</returns>
        private userView.serverPath getCurrentServerPath(object value)
        {
            userView.serverPath serverPath = (userView.serverPath)value;
            userView.serverPermission serverPermission = getCurrentServerPermission(serverPath.ServerPermission);
            return serverPermission != null && serverPermission.CurrentServerPath == serverPath ? serverPath : null;
        }
        /// <summary>
        /// 设置服务器端列表名称
        /// </summary>
        /// <param name="value">服务器端路径</param>
        private void setServerList(object value)
        {
            userView.serverPath serverPath = getCurrentServerPath(value);
            if (serverPath != null) setServerList(serverPath, true);
        }
        /// <summary>
        /// 清除服务器端列表名称
        /// </summary>
        private void clearServerList()
        {
            serverPathTextBox.Text = string.Empty;
            object selectItem = serverCheckedListBox.SelectedItem;
            int isClearCheckListName = 0;
            if (selectItem != null)
            {
                userView.serverListName listName = (userView.serverListName)selectItem;
                listName.ServerPath.SelectListName = listName;
                listName.ServerPath.CheckListNames.Clear();
                isClearCheckListName = 1;
            }
            foreach (userView.serverListName listName in serverCheckedListBox.CheckedItems)
            {
                if (isClearCheckListName == 0)
                {
                    isClearCheckListName = 1;
                    listName.ServerPath.CheckListNames.Clear();
                }
                listName.ServerPath.CheckListNames.Add(listName.ListName.LowerName);
            }
            serverCheckedListBox.Items.Clear();
            currentServerPath = null;
        }

        /// <summary>
        /// 设置服务器端列表名称
        /// </summary>
        /// <param name="serverPath">服务器端路径</param>
        /// <param name="isSetHistory">是否设置历史路径</param>
        private void setServerList(userView.serverPath serverPath, bool isSetHistory)
        {
            clearServerList();
            serverPathTextBox.Text = (currentServerPath = serverPath).Path.Name;
            userView.serverListName[] listNames = serverPath.ListNames;
            HashSet<hashString> checkExtensions = extensionFilterHash;
            bool isDirectory = serverDirectoryCheckBox.Checked;
            if (checkExtensions != null || !isDirectory)
            {
                list<userView.serverListName> nameList = new list<userView.serverListName>(listNames.Length);
                foreach (userView.serverListName listName in listNames)
                {
                    if (listName.ListName.IsFile ? (checkExtensions == null || checkExtensions.Contains(listName.ExtensionName)) : isDirectory)
                    {
                        nameList.UnsafeAdd(listName);
                    }
                }
                if (nameList.Count != listNames.Length) listNames = nameList.ToArray();
            }
            serverCheckedListBox.Items.AddRange(listNames);
            int index = 0;
            foreach (userView.serverListName listName in listNames)
            {
                if (serverPath.CheckListNames.Contains(listName.ListName.LowerName)) serverCheckedListBox.SetItemChecked(index, true);
                ++index;
            }
            refreshServerListNameButton.Enabled = true;
            allServerListNameButton.Enabled = changeServerListNameButton.Enabled = clearServerListNameButton.Enabled = listNames.Length != 0;
            deleteServerListNameButton.Enabled = listNames.Length != 0 && (serverPath.ServerPermission.PathPermission.Permission & permissionType.Write) != permissionType.None;
            uploadButton.Enabled = downloadButton.Enabled = currentLocalPath.Name != null;
            if (isSetHistory) serverPath.ServerPermission.HistoryPath.Set((currentServerPath = serverPath).Path);
            setServerPathButton(serverPath);
        }
        /// <summary>
        /// 设置本地路径
        /// </summary>
        /// <param name="path">本地路径</param>
        /// <param name="isSetHistory">是否设置历史路径</param>
        /// <param name="isRefresh">是否刷新</param>
        private void setLocalPath(lowerName path, bool isSetHistory, bool isRefresh)
        {
            DirectoryInfo pathInfo = null;
            try
            {
                for (pathInfo = new DirectoryInfo(path.Name); pathInfo != null && !pathInfo.Exists; pathInfo = pathInfo.Parent) ;
            }
            catch { }
            if (pathInfo != null)
            {
                lowerName localPath = currentLocalPath;
                pathComboBox.Text = currentLocalPath.Name = pathInfo.fullName();
                if (isRefresh || localPath.LowerName != currentLocalPath.LowerName)
                {
                    HashSet<hashString> checkNames = localPath.LowerName == currentLocalPath.LowerName ? clientCheckedListBox.CheckedItems.toGeneric<localListName>().getHash(value => (hashString)value.ListName.LowerName) : null;
                    HashSet<hashString> checkExtensions = extensionFilterHash;
                    DirectoryInfo[] directorys = clientDirectoryCheckBox.Checked ? pathInfo.GetDirectories() : nullValue<DirectoryInfo>.Array;
                    FileInfo[] files = pathInfo.GetFiles();
                    list<localListName> nameList = new list<localListName>(directorys.Length + files.Length);
                    foreach (DirectoryInfo directory in directorys) nameList.UnsafeAdd(new localListName(currentLocalPath, directory));
                    foreach (FileInfo file in files)
                    {
                        if (checkExtensions == null || isExtensionFilter(checkExtensions, file)) nameList.UnsafeAdd(new localListName(currentLocalPath, file));
                    }
                    localListName[] listNames = nameList.ToArray();
                    clientCheckedListBox.Items.Clear();
                    clientCheckedListBox.Items.AddRange(listNames);
                    if (checkNames != null)
                    {
                        int index = 0;
                        foreach (localListName listName in listNames)
                        {
                            if (checkNames.Contains(listName.ListName.LowerName)) clientCheckedListBox.SetItemChecked(index, true);
                            ++index;
                        }
                    }
                    refreshListNameButton.Enabled = true;
                    allListNameButton.Enabled = changeListNameButton.Enabled = clearListNameButton.Enabled = deleteListNameButton.Enabled = listNames.Length != 0;
                    uploadButton.Enabled = downloadButton.Enabled = currentServerPath != null;
                    if (isSetHistory) localHistoryPath.Set(currentLocalPath);
                    setPathButton(pathInfo);
                    parentPathButton.Enabled = pathInfo.Parent != null;
                    previousPathButton.Enabled = localHistoryPath.CurrentIndex > 1;
                    nextPathButton.Enabled = localHistoryPath.CurrentIndex != localHistoryPath.History.Count;
                }
            }
        }
        /// <summary>
        /// 更新本地目录信息
        /// </summary>
        /// <param name="directoryName">目录名称</param>
        private void onCreateDirectory(object directoryName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo((string)directoryName);
            if (currentLocalPath.LowerName == directoryInfo.Parent.fullName().toLower() && directoryInfo.Exists)
            {
                string name = directoryInfo.Name.toLower();
                localListName item = clientCheckedListBox.Items.toGeneric<localListName>().firstOrDefault(value => value.ListName.LowerName == name);
                if (item == null)
                {
                    clientCheckedListBox.Items.Add(new localListName(currentLocalPath, directoryInfo));
                    allListNameButton.Enabled = changeListNameButton.Enabled = clearListNameButton.Enabled = deleteListNameButton.Enabled = true;
                }
                else
                {
                    item.Set(directoryInfo);
                    clientCheckedListBox.Refresh();
                }
            }
        }
        /// <summary>
        /// 列表名称转字符串
        /// </summary>
        /// <param name="listName">列表名称</param>
        /// <returns></returns>
        private static string toString(server.listName listName)
        {
            if (listName.IsFile)
            {
                return listName.Name + " [" + listName.LastWriteTime.toString() + "] " + listName.Length.toString() + "B";
            }
            return "→ " + listName.Name + " [" + listName.LastWriteTime.toString() + "]";
        }
        /// <summary>
        /// 选择用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectUser(object sender, EventArgs e)
        {
            object selectItem = userComboBox.SelectedItem;
            if (selectItem != null) setUser((user)selectItem);
        }
        /// <summary>
        /// 获取输入用户
        /// </summary>
        /// <returns>输入用户,失败返回null</returns>
        private user getInputUser()
        {
            string host = hostTextBox.Text.Trim(), userName = userNameTextBox.Text.Trim();
            if (host.Length != 0 && userName.Length != 0)
            {
                ushort port;
                if (ushort.TryParse(portTextBox.Text.Trim(), out port) && port != 0) return new user { Host = host, Port = port, Name = userName };
            }
            return null;
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeUserButton_Click(object sender, EventArgs e)
        {
            user user = getInputUser();
            if (user != null)
            {
                user cacheUser = user.Table.Cache.Values.firstOrDefault(value => value.Equals(user));
                if (cacheUser == null) closeCurrent(user);
                else if (close(cacheUser))
                {
                    if (user.Table.Delete(cacheUser.Id) == null) addError("错误：用户删除失败 " + cacheUser.ToString());
                    else addMessage("成功删除用户 " + cacheUser.ToString());
                }
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            user user = getInputUser();
            if (user != null)
            {
                user cacheUser = user.Table.Cache.Values.firstOrDefault(value => value.Equals(user));
                if (cacheUser == null) closeCurrent(user);
                else close(cacheUser);
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="user"></param>
        private bool close(user user)
        {
            userView userView;
            Monitor.Enter(userViewLock);
            userViews.TryGetValue(user.Id, out userView);
            Monitor.Exit(userViewLock);
            if (userView == null) return true;
            bool isUpload = uploadQueue.IsUserView(userView), isDownload = downloadQueue.IsUserView(userView);
            if (!isUpload && !isDownload && MessageBox.Show((isUpload ? @"上传操作未完成
" : null) + (isDownload ? @"下载操作未完成
" : null) + "是否确定关闭连接？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Monitor.Enter(userViewLock);
                try
                {
                    userViews.Remove(user.Id);
                }
                finally { Monitor.Exit(userViewLock); }
                pub.Dispose(ref userView);
                if (currentUserId == user.Id) setUser(user);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="user"></param>
        private void closeCurrent(user user)
        {
            userView userView;
            Monitor.Enter(currentClientLock);
            try
            {
                if (currentClients.TryGetValue(user, out userView)) currentClients.Remove(user);
            }
            finally { Monitor.Exit(currentClientLock); }
            if (userView == null) addMessage("提示：未找到用户 " + user.ToString());
            pub.Dispose(ref userView);
        }
        /// <summary>
        /// 客户端连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientButton_Click(object sender, EventArgs e)
        {
            user user = getInputUser();
            if (user != null)
            {
                user.Password = passwordTextBox.Text.Trim();
                userView userView = null;
                user cacheUser = user.Table.Cache.Values.firstOrDefault(value => value.Equals(user));
                if (cacheUser == null)
                {
                    user.Id = Interlocked.Decrement(ref currentClientIdentity);
                    Monitor.Enter(currentClientLock);
                    try
                    {
                        if (!currentClients.ContainsKey(user))
                        {
                            currentClients.Add(user, userView = new userView { Form = this, UserId = user.Id, NewUser = user });
                        }
                    }
                    finally { Monitor.Exit(currentClientLock); }
                    if (userView != null)
                    {
                        currentUserId = user.Id;
                        fastCSharp.threading.threadPool.TinyPool.Start(userView.CreateClient, false);
                    }
                }
                else
                {
                    Monitor.Enter(userViewLock);
                    try
                    {
                        if (!userViews.TryGetValue(cacheUser.Id, out userView))
                        {
                            userViews.Add(cacheUser.Id, userView = new userView { Form = this, UserId = cacheUser.Id });
                        }
                    }
                    finally { Monitor.Exit(userViewLock); }
                    currentUserId = cacheUser.Id;
                    if (user.Password != cacheUser.Password) cacheUser.ReworkPassword(user.Password);
                    fastCSharp.threading.threadPool.TinyPool.Start(userView.CreateClient, true);
                }
            }
        }
        /// <summary>
        /// 删除正在连接的客户端集合
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        private void removeCurrent(user user, int userId)
        {
            if (userId < 0)
            {
                Monitor.Enter(userViewLock);
                try
                {
                    userViews.Remove(userId);
                }
                finally { Monitor.Exit(userViewLock); }
            }
            Monitor.Enter(currentClientLock);
            try
            {
                currentClients.Remove(user);
            }
            finally { Monitor.Exit(currentClientLock); }
        }
        /// <summary>
        /// 修改本地路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePath(object sender, EventArgs e)
        {
            changePath();
        }
        /// <summary>
        /// 修改本地路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePath(object sender, KeyPressEventArgs e)
        {
            changePath();
        }
        /// <summary>
        /// 修改本地路径
        /// </summary>
        private void changePath()
        {
            setLocalPath(new lowerName { Name = pathComboBox.Text.Trim() }, true, false);
        }
        /// <summary>
        /// 选择路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectPath(object sender, EventArgs e)
        {
            object selectItem = pathComboBox.SelectedItem;
            if (selectItem != null) setLocalPath(new lowerName { Name = (string)selectItem }, true, false);
        }
        /// <summary>
        /// 选择路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathButton_Click(object sender, EventArgs e)
        {
            string path = pathComboBox.Text.Trim();
            if (Directory.Exists(path)) folderBrowserDialog.SelectedPath = path;
            folderBrowserDialog.ShowDialog();
            if ((path = folderBrowserDialog.SelectedPath).length() != 0) setLocalPath(new lowerName { Name = path }, true, false);
        }
        /// <summary>
        /// 选择服务器端权限路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectServerPermission(object sender, EventArgs e)
        {
            object selectItem = serverPermissionComboBox.SelectedItem;
            if (selectItem != null) setServerPermission(selectItem);
        }
        /// <summary>
        /// 全选本地列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allListNameButton_Click(object sender, EventArgs e)
        {
            setCheckedListBox(clientCheckedListBox, true);
        }
        /// <summary>
        /// 反选本地列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeListNameButton_Click(object sender, EventArgs e)
        {
            changeCheckedListBox(clientCheckedListBox);
        }
        /// <summary>
        /// 清除本地列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearListNameButton_Click(object sender, EventArgs e)
        {
            setCheckedListBox(clientCheckedListBox, false);
        }
        /// <summary>
        /// 全选服务器端列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allServerListNameButton_Click(object sender, EventArgs e)
        {
            setCheckedListBox(serverCheckedListBox, true);
        }
        /// <summary>
        /// 反选服务器端列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeServerListNameButton_Click(object sender, EventArgs e)
        {
            changeCheckedListBox(serverCheckedListBox);
        }
        /// <summary>
        /// 清除服务器端列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearServerListNameButton_Click(object sender, EventArgs e)
        {
            setCheckedListBox(serverCheckedListBox, false);
        }
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="isChecked">是否选择</param>
        private static void setCheckedListBox(CheckedListBox listBox, bool isChecked)
        {
            for (int index = listBox.Items.Count; index != 0; listBox.SetItemChecked(--index, isChecked)) ;
        }
        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="listBox"></param>
        private static void changeCheckedListBox(CheckedListBox listBox)
        {
            for (int index = listBox.Items.Count; --index >= 0; listBox.SetItemChecked(index, !listBox.GetItemChecked(index))) ;
        }
        /// <summary>
        /// 删除本地列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteListNameButton_Click(object sender, EventArgs e)
        {
            CheckedListBox.CheckedItemCollection listNames = clientCheckedListBox.CheckedItems;
            if (listNames.Count != 0 && MessageBox.Show("是否确定删除本地文件？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lowerName currentPath = default(lowerName);
                foreach (localListName listName in listNames)
                {
                    currentPath = listName.Path;
                    if (listName.ListName.IsFile)
                    {
                        string fileName = listName.Path.Name + listName.ListName.Name;
                        if (File.Exists(fileName))
                        {
                            try
                            {
                                File.Delete(fileName);
                            }
                            catch (Exception error)
                            {
                                log.Error.Add(error, null, false);
                                addError("错误：本地文件删除失败 " + fileName);
                            }
                        }
                    }
                    else
                    {
                        string path = listName.Path.Name + listName.ListName.Name;
                        if (Directory.Exists(path))
                        {
                            try
                            {
                                Directory.Delete(path, true);
                            }
                            catch (Exception error)
                            {
                                log.Error.Add(error, null, false);
                                addError("错误：本地目录删除失败 " + path);
                            }
                        }
                    }
                }
                setLocalPath(currentPath, true, true);
            }
        }
        /// <summary>
        /// 删除服务器端列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteServerListNameButton_Click(object sender, EventArgs e)
        {
            CheckedListBox.CheckedItemCollection listNames = serverCheckedListBox.CheckedItems;
            if (listNames.Count != 0 && MessageBox.Show("是否确定删除远程文件？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                userView.serverPath serverPath = null;
                server.listName[] serverListNames = new server.listName[listNames.Count];
                int index = 0;
                foreach (userView.serverListName listName in listNames)
                {
                    serverPath = listName.ServerPath;
                    serverListNames[index++] = listName.ListName;
                }
                serverPath.ServerPermission.View.Client.getBackupIdentity(new deleteIdentityGetter { ServerPath = serverPath, ListNames = serverListNames }.OnGetIdentity);
            }
        }
        /// <summary>
        /// 本地父级目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void parentPathButton_Click(object sender, EventArgs e)
        {
            if (currentLocalPath.Name != null)
            {
                lowerName localPath = currentLocalPath;
                DirectoryInfo parentInfo = new DirectoryInfo(currentLocalPath.Name).Parent;
                if (parentInfo != null)
                {
                    string lastPath = currentServerPath != null && pathCheckBox.Checked ? currentServerPath.LastPath : null;
                    setLocalPath(new lowerName { Name = parentInfo.FullName }, true, true);
                    setPathButton(parentInfo);
                    if (isPath(lastPath, localPath.LowerName))
                    {
                        currentServerPath.ServerPermission.CurrentPath.Name = currentServerPath.ParentPath;
                        setServerPermission(currentServerPath.ServerPermission, true);
                    }
                }
            }
        }
        /// <summary>
        /// 上一个本地历史路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previousPathButton_Click(object sender, EventArgs e)
        {
            lowerName path = localHistoryPath.Previous;
            if (path.Name != null)
            {
                setLocalPath(path, false, true);
                setPathButton(new DirectoryInfo(path.Name));
                localHistoryPath.Previous = currentLocalPath;
            }
        }
        /// <summary>
        /// 下一个本地历史路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextPathButton_Click(object sender, EventArgs e)
        {
            lowerName path = localHistoryPath.Next;
            if (path.Name != null)
            {
                setLocalPath(path, false, true);
                setPathButton(new DirectoryInfo(path.Name));
                localHistoryPath.Next = currentLocalPath;
            }
        }
        /// <summary>
        /// 服务器端父级目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private unsafe void parentServerPathButton_Click(object sender, EventArgs e)
        {
            if (currentServerPath != null && currentServerPath.Path.Name.Length != 0)
            {
                string lastPath = currentLocalPath.Name != null && pathCheckBox.Checked ? currentServerPath.LastPath : null;
                currentServerPath.ServerPermission.CurrentPath.Name = currentServerPath.ParentPath;
                setServerPermission(currentServerPath.ServerPermission, true);
                setServerPathButton(currentServerPath);
                if (isPath(lastPath, currentLocalPath.LowerName))
                {
                    setLocalPath(new lowerName { Name = currentLocalPath.Name.Substring(0, currentLocalPath.Name.Length - lastPath.Length) }, true, true);
                }
            }
        }
        /// <summary>
        /// 路径匹配
        /// </summary>
        /// <param name="lastPath"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        private static bool isPath(string lastPath, string localPath)
        {
            return lastPath != null && (lastPath.Length == 0 || (localPath.Length > lastPath.Length
                    && localPath[localPath.Length - lastPath.Length - 1] == System.IO.Path.DirectorySeparatorChar
                    && localPath.EndsWith(lastPath, StringComparison.Ordinal)));
        }
        /// <summary>
        /// 上一个服务器端目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previousServerPathButton_Click(object sender, EventArgs e)
        {
            if (currentServerPath != null)
            {
                userView.serverPermission serverPermission = currentServerPath.ServerPermission;
                lowerName path = serverPermission.HistoryPath.Previous;
                if (path.Name != null)
                {
                    serverPermission.CurrentPath = path;
                    if (setServerPermission(serverPermission, false))
                    {
                        serverPermission.HistoryPath.Previous = path;
                        setServerPathButton(currentServerPath);
                    }
                }
            }
        }
        /// <summary>
        /// 下一个服务器端目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextServerPathButton_Click(object sender, EventArgs e)
        {
            if (currentServerPath != null)
            {
                userView.serverPermission serverPermission = currentServerPath.ServerPermission;
                lowerName path = serverPermission.HistoryPath.Next;
                if (path.Name != null)
                {
                    serverPermission.CurrentPath = path;
                    if (setServerPermission(serverPermission, false))
                    {
                        serverPermission.HistoryPath.Next = path;
                        setServerPathButton(currentServerPath);
                    }
                }
            }
        }
        /// <summary>
        /// 设置本地路径按钮状态
        /// </summary>
        /// <param name="pathInfo"></param>
        private void setPathButton(DirectoryInfo pathInfo)
        {
            parentPathButton.Enabled = pathInfo.Parent != null;
            previousPathButton.Enabled = localHistoryPath.CurrentIndex > 1;
            nextPathButton.Enabled = localHistoryPath.CurrentIndex != localHistoryPath.History.Count;
        }
        /// <summary>
        /// 设置服务器端路径按钮状态
        /// </summary>
        /// <param name="serverPath"></param>
        private void setServerPathButton(userView.serverPath serverPath)
        {
            parentServerPathButton.Enabled = serverPath.Path.Name.Length != 0;
            previousServerPathButton.Enabled = serverPath.ServerPermission.HistoryPath.CurrentIndex > 1;
            nextServerPathButton.Enabled = serverPath.ServerPermission.HistoryPath.CurrentIndex != serverPath.ServerPermission.HistoryPath.History.Count;
        }
        /// <summary>
        /// 刷新本地路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshListNameButton_Click(object sender, EventArgs e)
        {
            if (currentLocalPath.Name != null) setLocalPath(currentLocalPath, false, true);
        }
        /// <summary>
        /// 刷新服务器端列表名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshServerListNameButton_Click(object sender, EventArgs e)
        {
            if (currentServerPath != null)
            {
                currentServerPath.IsChange = 1;
                setServerPermission(currentServerPath.ServerPermission, false);
            }
        }
        /// <summary>
        /// 扩展名过滤更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkExtensionFilter(object sender, EventArgs e)
        {
            if (isChangeExtensionFilter)
            {
                refreshListNameButton_Click(null, null);
                refreshServerListNameButton_Click(null, null);
            }
        }
        /// <summary>
        /// 进入目录或者上传文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientEnter(object sender, EventArgs e)
        {
            object selectItem = clientCheckedListBox.SelectedItem;
            if (selectItem != null)
            {
                localListName listName = (localListName)selectItem;
                if (listName.ListName.IsFile)
                {
                    if (currentServerPath != null)
                    {
                        currentServerPath.ServerPermission.View.Client.getBackupIdentity(new uploadIdentityGetter
                        {
                            ServerPath = currentServerPath,
                            ListNames = new localListName[] { listName },
                            ExtensionFilter = autoCheckBox.Checked ? extensionFilterTextBox.Text.Trim() : null,
                            IsTimeVersion = timeVersionCheckBox.Checked
                        }.OnGetIdentity);
                    }
                }
                else
                {
                    string lastPath = currentServerPath != null && pathCheckBox.Checked ? currentServerPath.LastPath ?? string.Empty : null;
                    setLocalPath(new lowerName { Name = listName.Path.Name + listName.ListName.Name }, true, true);
                    if (isPath(lastPath, listName.Path.LowerName)
                        && (currentServerPath.ListNames.any(value => !value.ListName.IsFile && value.ListName.LowerName == listName.ListName.LowerName)
                        || serverCheckedListBox.Items.toGeneric<userView.serverListName>().any(value => !value.ListName.IsFile && value.ListName.LowerName == listName.ListName.LowerName)))
                    {
                        currentServerPath.ServerPermission.CurrentPath.Name = currentServerPath.Path.Name + listName.ListName.Name + directory.DirectorySeparator;
                        setServerPermission(currentServerPath.ServerPermission, true);
                    }
                }
            }
        }
        /// <summary>
        /// 进入目录或者下载文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private unsafe void serverEnter(object sender, EventArgs e)
        {
            object selectItem = serverCheckedListBox.SelectedItem;
            if (selectItem != null)
            {
                userView.serverListName listName = (userView.serverListName)selectItem;
                if (listName.ListName.IsFile)
                {
                    if (currentLocalPath.Name != null) listName.ServerPath.Download(listName, currentLocalPath, null, autoCheckBox.Checked ? extensionFilterTextBox.Text.Trim() : null, timeVersionCheckBox.Checked);
                }
                else
                {
                    string lastPath = currentLocalPath.Name != null && pathCheckBox.Checked ? listName.ServerPath.LastPath ?? string.Empty : null;
                    listName.ServerPath.ServerPermission.CurrentPath.Name = listName.ServerPath.Path.Name + listName.ListName.Name + fastCSharp.directory.DirectorySeparator;
                    setServerPermission(listName.ServerPath.ServerPermission, true);
                    if (isPath(lastPath, currentLocalPath.LowerName))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(currentLocalPath.Name);
                        if (directoryInfo.Exists)
                        {
                            int nameLength = (lastPath = listName.ListName.Name).Length;
                            fixed (char* nameFixed = lastPath)
                            {
                                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                                {
                                    string name = directory.Name;
                                    if (name.Length == nameLength && unsafer.String.EqualCase(name, nameFixed, nameLength))
                                    {
                                        setLocalPath(new lowerName { Name = currentLocalPath.Name + name }, true, true);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 批量上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (currentServerPath != null)
            {
                CheckedListBox.CheckedItemCollection listNames = clientCheckedListBox.CheckedItems;
                if (listNames.Count != 0)
                {
                    currentServerPath.ServerPermission.View.Client.getBackupIdentity(new uploadIdentityGetter 
                    {
                        ServerPath = currentServerPath,
                        ListNames = listNames.toGeneric<localListName>().getArray().getSort(value => value.ListName.Length),
                        ExtensionFilterHash = extensionFilterHash,
                        ExtensionFilter = autoCheckBox.Checked ? extensionFilterTextBox.Text.Trim() : null,
                        IsTimeVersion = timeVersionCheckBox.Checked
                    }.OnGetIdentity);
                }
            }
        }
        /// <summary>
        /// 服务器端文件信息
        /// </summary>
        /// <param name="value">文件上传信息</param>
        private void onUploadFile(object value)
        {
            uploadInfo uploadInfo = (uploadInfo)value;
            string fileName = uploadInfo.ListName.Path.Name + uploadInfo.ListName.ListName.Name;
            if (uploadInfo.FileState == server.fileState.Success)
            {
                if (uploadInfo.ListName.Path.LowerName == currentLocalPath.LowerName)
                {
                    int index = 0;
                    foreach (localListName listName in clientCheckedListBox.Items)
                    {
                        if (listName.ListName.LowerName == uploadInfo.ListName.ListName.LowerName)
                        {
                            clientCheckedListBox.SetItemChecked(index, false);
                            break;
                        }
                        ++index;
                    }
                }
                if (uploadInfo.ServerPath == currentServerPath)
                {
                    userView.serverListName item = serverCheckedListBox.Items.toGeneric<userView.serverListName>()
                        .firstOrDefault(listName => listName.ListName.LowerName == uploadInfo.ListName.ListName.LowerName);
                    if (item == null)
                    {
                        serverCheckedListBox.Items.Add(new userView.serverListName { ServerPath = uploadInfo.ServerPath, ListName = uploadInfo.ListName.ListName });
                        allServerListNameButton.Enabled = changeServerListNameButton.Enabled = clearServerListNameButton.Enabled = deleteServerListNameButton.Enabled = true;
                    }
                    else
                    {
                        item.ListName = uploadInfo.ListName.ListName;
                        clientCheckedListBox.Refresh();
                    }
                }
                addMessage("文件上传成功 " + fileName);
            }
            else addError("错误：服务器端文件操作失败[" + uploadInfo.FileState.ToString() + "] " + fileName);
        }
        /// <summary>
        /// 服务器端目录信息
        /// </summary>
        /// <param name="value">文件上传信息</param>
        private void onUploadDirectory(object value)
        {
            uploadInfo uploadInfo = (uploadInfo)value;
            if (uploadInfo.ServerPath == currentServerPath)
            {
                userView.serverListName item = serverCheckedListBox.Items.toGeneric<userView.serverListName>()
                    .firstOrDefault(listName => listName.ListName.LowerName == uploadInfo.ListName.ListName.LowerName);
                if (item == null)
                {
                    serverCheckedListBox.Items.Add(new userView.serverListName { ServerPath = uploadInfo.ServerPath, ListName = uploadInfo.ListName.ListName });
                    allServerListNameButton.Enabled = changeServerListNameButton.Enabled = clearServerListNameButton.Enabled = deleteServerListNameButton.Enabled = true;
                }
            }
        }
        /// <summary>
        /// 批量下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadButton_Click(object sender, EventArgs e)
        {
            if (currentLocalPath.Name != null)
            {
                CheckedListBox.CheckedItemCollection listNames = serverCheckedListBox.CheckedItems;
                if (listNames.Count != 0)
                {
                    bool isAutoPath = autoCheckBox.Checked;
                    currentServerPath.Download(listNames.toGeneric<userView.serverListName>().getArray(), currentLocalPath, extensionFilterHash, isAutoPath ? extensionFilterTextBox.Text.Trim() : null, false, timeVersionCheckBox.Checked);
                }
            }
        }
        /// <summary>
        /// 更新本地文件信息
        /// </summary>
        /// <param name="value">文件下载信息</param>
        private void onDownloadFile(object value)
        {
            downloadInfo downloadInfo = (downloadInfo)value;
            string fileName = downloadInfo.LocalPath.Name + downloadInfo.ListName.ListName.Name;
            if (downloadInfo.LocalPath.LowerName == currentLocalPath.LowerName)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                {
                    localListName item = clientCheckedListBox.Items.toGeneric<localListName>()
                        .firstOrDefault(listName => listName.ListName.LowerName == downloadInfo.ListName.ListName.LowerName);
                    if (item == null)
                    {
                        clientCheckedListBox.Items.Add(new localListName(currentLocalPath, fileInfo));
                        allListNameButton.Enabled = changeListNameButton.Enabled = clearListNameButton.Enabled = deleteListNameButton.Enabled = true;
                    }
                    else
                    {
                        item.Set(fileInfo);
                        clientCheckedListBox.Refresh();
                    }
                }
            }
            if (downloadInfo.FileState == server.fileState.Success)
            {
                if (downloadInfo.ServerPath == currentServerPath)
                {
                    int index = 0;
                    foreach (userView.serverListName listName in serverCheckedListBox.Items)
                    {
                        if (listName.ListName.LowerName == downloadInfo.ListName.ListName.LowerName)
                        {
                            serverCheckedListBox.SetItemChecked(index, false);
                            break;
                        }
                        ++index;
                    }
                }
                addMessage("文件下载成功 " + fileName);
            }
            else addError("错误：服务器端文件操作失败[" + downloadInfo.FileState.ToString() + "] " + fileName);
        }
        /// <summary>
        /// 设置最大上传线程数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setMaxUploadThreadCount(object sender, EventArgs e)
        {
            uploadQueue.SetMaxThreadCount(parseThreadCount(uploadThreadTextBox, 99));
        }
        /// <summary>
        /// 设置最大下载线程数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setMaxDownloadThreadCount(object sender, EventArgs e)
        {
            downloadQueue.SetMaxThreadCount(parseThreadCount(downloadThreadTextBox, 99));
        }
        /// <summary>
        /// 解析线程数量
        /// </summary>
        /// <param name="textbox">输入框</param>
        /// <param name="defaultThreadCount">默认线程数量</param>
        /// <returns>线程数量</returns>
        private int parseThreadCount(TextBox textbox, int defaultThreadCount)
        {
            int threadCount;
            if (int.TryParse(textbox.Text, out threadCount))
            {
                if (threadCount <= 0)
                {
                    addMessage("线程数量不能为 " + threadCount.toString());
                    textbox.Text = (threadCount = defaultThreadCount).toString();
                }
            }
            else
            {
                addMessage("线程数量不能为 " + textbox.Text);
                textbox.Text = (threadCount = defaultThreadCount).toString();
            }
            return threadCount;
        }
        /// <summary>
        /// 扩展名过滤
        /// </summary>
        /// <param name="extensionFilter"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static bool isExtensionFilter(HashSet<hashString> extensionFilter, FileInfo file)
        {
            string name = file.Extension;
            return extensionFilter.Contains(name.length() > 1 ? new subString(name.toLower(), 1) : new subString(string.Empty));

        }
    }
}
