using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using fastCSharp.code.cSharp;
using fastCSharp.io;
using fastCSharp.threading;
using System.Security.Cryptography;
using fastCSharp.net;

namespace fastCSharp.demo.fileTransferServer
{
    /// <summary>
    /// 文件传输服务端
    /// </summary>
    [fastCSharp.code.cSharp.tcpServer(Service = "fileTransfer", IsIdentityCommand = true, IsServerAsynchronousReceive = false, IsCompress = true, Host = "127.0.0.1", Port = 12345, IsRememberIdentityCommand = false)]
    public partial class server
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="user">用户</param>
        /// <returns>是否成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsVerifyMethod = true, IsServerSynchronousTask = false, InputParameterMaxLength = 1024, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private bool login(fastCSharp.net.tcp.commandServer.socket client, string userName, byte[] password, DateTime verifyTime)
        {
            user cacheUser = user.Table.Cache.Get(userName);
            if (cacheUser != null)
            {
                if (cacheUser.LoginVerify(userName, password, verifyTime))
                {
                    client.ClientUserInfo = new user { Name = userName, Password = cacheUser.Password };
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 备份路径编号
        /// </summary>
        private static int backupIdentity;
        /// <summary>
        /// 获取备份路径编号
        /// </summary>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private int getBackupIdentity()
        {
            return Interlocked.Increment(ref backupIdentity);
        }
        /// <summary>
        /// 路径权限
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct pathPermission
        {
            /// <summary>
            /// 操作路径
            /// </summary>
            public string Path;
            /// <summary>
            /// 权限类型
            /// </summary>
            public permissionType Permission;
            /// <summary>
            /// 设置路径权限
            /// </summary>
            /// <param name="path">操作路径</param>
            /// <param name="permission">权限类型</param>
            public void Set(string path, permissionType permission)
            {
                Path = path;
                Permission = permission;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return ((Permission & permissionType.Read) == permissionType.None ? null : "R") + ((Permission & permissionType.Write) == permissionType.None ? null : "W") + (Permission == permissionType.List ? null : " | ") + Path;
            }
        }
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <returns>权限列表</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private pathPermission[] getPermissions(fastCSharp.net.tcp.commandServer.socket client)
        {
            if (verify(client))
            {
                ICollection<permission> permissions = permission.UserCache.GetCache(((user)client.ClientUserInfo).Name);
                pathPermission[] paths = new pathPermission[permissions.Count];
                int index = 0;
                foreach (permission value in permissions) paths[index++].Set(value.Path, value.Type);
                return paths;
            }
            return null;
        }
        /// <summary>
        /// 列表名称
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
        public struct listName
        {
            /// <summary>
            /// 最后修改时间
            /// </summary>
            public DateTime LastWriteTime;
            /// <summary>
            /// 是否文件,long.MinValue表示目录
            /// </summary>
            public long Length;
            /// <summary>
            /// 名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 小写名称
            /// </summary>
            [fastCSharp.code.ignore]
            private string lowerName;
            /// <summary>
            /// 小写名称
            /// </summary>
            public string LowerName
            {
                get
                {
                    if (lowerName == null && Name != null) lowerName = Name.ToLower();
                    return lowerName;
                }
            }
            /// <summary>
            /// 是否文件,false表示目录
            /// </summary>
            public bool IsFile
            {
                get { return Length != long.MinValue; }
            }
            /// <summary>
            /// 列表名称
            /// </summary>
            /// <param name="name">名称</param>
            public void Set(string name)
            {
                Name = name;
                lowerName = null;
            }
            /// <summary>
            /// 列表名称
            /// </summary>
            /// <param name="name">名称</param>
            /// <param name="lastWriteTime">最后修改时间</param>
            /// <param name="length">文件长度</param>
            public void Set(string name, DateTime lastWriteTime, long length)
            {
                Name = name;
                lowerName = null;
                Length = length;
                LastWriteTime = lastWriteTime;
            }
        }
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="path">文件路径</param>
        /// <returns>文件列表</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private listName[] list(fastCSharp.net.tcp.commandServer.socket client, string path)
        {
            if (verify(client))
            {
                DirectoryInfo pathInfo = new DirectoryInfo(path);
                if (pathInfo.Exists && verify(client, pathInfo.fullName().toLower(), permissionType.List))
                {
                    return getListName(pathInfo);
                }
            }
            return null;
        }
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="pathInfo">目录信息</param>
        /// <returns>文件列表</returns>
        private static listName[] getListName(DirectoryInfo pathInfo)
        {
            DirectoryInfo[] directorys = pathInfo.GetDirectories();
            FileInfo[] files = pathInfo.GetFiles();
            listName[] listNames = new listName[directorys.Length + files.Length];
            int index = 0;
            foreach (DirectoryInfo directory in directorys) listNames[index++].Set(directory.Name, directory.LastWriteTimeUtc, long.MinValue);
            foreach (FileInfo file in files) listNames[index++].Set(file.Name, file.LastWriteTimeUtc, file.Length);
            return listNames;
        }
        /// <summary>
        /// 目录删除器
        /// </summary>
        private struct directoryDeleter
        {
            /// <summary>
            /// 客户端标识
            /// </summary>
            public fastCSharp.net.tcp.commandServer.socket Client;
            /// <summary>
            /// 备份路径编号
            /// </summary>
            public int BackupIdentity;
            /// <summary>
            /// 删除目录
            /// </summary>
            /// <param name="directory">目录信息</param>
            public bool Delete(DirectoryInfo directory)
            {
                permission permission = getPermission(Client, directory.fullName().toLower());
                if (verify(permission, permissionType.Write))
                {
                    bool isDelete = true;
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        try
                        {
                            permission.Backup(file, BackupIdentity);
                        }
                        catch (Exception error)
                        {
                            log.Error.Add(error, null, false);
                            isDelete = false;
                        }
                    }
                    foreach (DirectoryInfo nextDirectory in directory.GetDirectories()) isDelete &= Delete(nextDirectory);
                    if (isDelete)
                    {
                        directory.Delete();
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// 当前删除文件名称集合
        /// </summary>
        private readonly HashSet<hashString> deleteFileNames = hashSet.CreateHashString();
        /// <summary>
        /// 当前删除目录名称集合
        /// </summary>
        private readonly HashSet<hashString> deleteDirectoryNames = hashSet.CreateHashString();
        /// <summary>
        /// 删除集合访问锁
        /// </summary>
        private readonly object deleteLock = new object();
        /// <summary>
        /// 删除文件列表
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="path">文件路径</param>
        /// <param name="listNames">文件列表</param>
        /// <returns>删除后的文件列表</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private listName[] delete(fastCSharp.net.tcp.commandServer.socket client, string path, listName[] listNames, int backupIdentity)
        {
            if (verify(client))
            {
                DirectoryInfo pathInfo = new DirectoryInfo(path);
                if (pathInfo.Exists)
                {
                    permission permission = getPermission(client, path = pathInfo.fullName().toLower());
                    if (verify(permission, permissionType.Write))
                    {
                        directoryDeleter directoryDeleter = new directoryDeleter { Client = client, BackupIdentity = backupIdentity };
                        Monitor.Enter(deleteLock);
                        try
                        {
                            deleteFileNames.Clear();
                            deleteDirectoryNames.Clear();
                            foreach (listName listName in listNames) (listName.IsFile ? deleteFileNames : deleteDirectoryNames).Add(listName.LowerName);
                            if (deleteFileNames.Count != 0)
                            {
                                foreach (FileInfo file in pathInfo.GetFiles())
                                {
                                    if (deleteFileNames.Contains(file.Name.toLower()))
                                    {
                                        try
                                        {
                                            permission.Backup(file, backupIdentity);
                                        }
                                        catch (Exception error)
                                        {
                                            log.Error.Add(error, null, false);
                                        }
                                    }
                                }
                            }
                            if (deleteDirectoryNames.Count != 0)
                            {
                                foreach (DirectoryInfo directory in pathInfo.GetDirectories())
                                {
                                    if (deleteDirectoryNames.Contains(directory.Name.toLower()))
                                    {
                                        try
                                        {
                                            directoryDeleter.Delete(directory);
                                        }
                                        catch (Exception error)
                                        {
                                            log.Error.Add(error, null, false);
                                        }
                                    }
                                }
                            }
                        }
                        finally { Monitor.Exit(deleteLock); }
                        return getListName(pathInfo);
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 文件操作状态
        /// </summary>
        public enum fileState : byte
        {
            /// <summary>
            /// 无权限
            /// </summary>
            NoPermission,
            /// <summary>
            /// 异常
            /// </summary>
            Exception,
            /// <summary>
            /// 文件长度错误
            /// </summary>
            LengthError,
            /// <summary>
            /// 文件未找到
            /// </summary>
            FileNotFound,
            /// <summary>
            /// 时间版本不匹配
            /// </summary>
            TimeVersionError,
            /// <summary>
            /// 操作成功
            /// </summary>
            Success,
        }
        /// <summary>
        /// 文件上传器
        /// </summary>
        private sealed class uploader
        {
            /// <summary>
            /// 权限路径
            /// </summary>
            private permission permission;
            /// <summary>
            /// 传输结束委托
            /// </summary>
            private Func<returnValue<fileState>, bool> onUpload;
            /// <summary>
            /// 客户端文件流
            /// </summary>
            private Stream fileStream;
            /// <summary>
            /// 列表名称
            /// </summary>
            private listName listName;
            /// <summary>
            /// 数据缓冲区
            /// </summary>
            private byte[] buffer;
            /// <summary>
            /// 上传文件
            /// </summary>
            private Action uploadHandle;
            /// <summary>
            /// 备份路径编号
            /// </summary>
            private int backupIdentity;
            /// <summary>
            /// 是否匹配时间版本
            /// </summary>
            private bool isTimeVersion;
            /// <summary>
            /// 文件上传器
            /// </summary>
            private uploader()
            {
                uploadHandle = upload;
                buffer = new byte[fastCSharp.config.tcpCommand.Default.BigBufferSize];
            }
            /// <summary>
            /// 上传文件
            /// </summary>
            private void upload()
            {
                fileState fileState = fileState.Exception;
                FileStream writeStream = null;
                int isWriteTime = 0;
                try
                {
                    FileInfo fileInfo = new FileInfo(listName.Name);
                    if (fileInfo.Exists)
                    {
                        if (isTimeVersion && fileInfo.LastWriteTimeUtc > listName.LastWriteTime) fileState = server.fileState.TimeVersionError;
                        else if (fileInfo.LastWriteTimeUtc == listName.LastWriteTime && listName.Length >= fileInfo.Length)
                        {
                            writeStream = new FileStream(listName.Name, FileMode.Open, FileAccess.Write, FileShare.None);
                            writeStream.Seek(fileInfo.Length, SeekOrigin.Begin);
                        }
                        else permission.Backup(fileInfo, backupIdentity);
                    }
                    if (fileState != server.fileState.TimeVersionError)
                    {
                        if (writeStream == null) writeStream = new FileStream(listName.Name, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        isWriteTime = 1;
                        long position = writeStream.Position, length = fileStream.Length;
                        if (position < length)
                        {
                            if (position != 0) fileStream.Seek(position, SeekOrigin.Begin);
                            while (position < length)
                            {
                                int read = fileStream.Read(buffer, 0, buffer.Length);
                                if (read > 0)
                                {
                                    writeStream.Write(buffer, 0, read);
                                    position += read;
                                }
                                else break;
                            }
                        }
                        fileState = position == length ? fileState.Success : fileState.LengthError;
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Exception exception = null;
                pub.Dispose(ref writeStream, ref exception);
                if (exception != null)
                {
                    log.Error.Add(exception, null, false);
                    if (fileState == fileState.Success) fileState = fileState.Exception;
                }
                if (isWriteTime != 0)
                {
                    try
                    {
                        new FileInfo(listName.Name).LastWriteTimeUtc = listName.LastWriteTime;
                    }
                    catch (Exception error)
                    {
                        log.Error.Add(error, null, false);
                    }
                }
                push(fileState);
            }
            /// <summary>
            /// 文件操作结束
            /// </summary>
            /// <param name="fileState">文件操作状态</param>
            private void push(fileState fileState)
            {
                Func<returnValue<fileState>, bool> onUpload = this.onUpload;
                this.onUpload = null;
                pub.Dispose(ref fileStream);
                typePool<uploader>.PushNotNull(this);
                onUpload(fileState);
            }
            /// <summary>
            /// 获取文件上传器
            /// </summary>
            /// <param name="permission">权限路径</param>
            /// <param name="onUpload">上传结束委托</param>
            /// <param name="listName">列表名称</param>
            /// <param name="fileStream">文件流</param>
            /// <returns>文件上传器</returns>
            public static Action GetUpload(permission permission, Func<returnValue<fileState>, bool> onUpload
                , listName listName, Stream fileStream, int backupIdentity, bool isTimeVersion)
            {
                uploader uploader = typePool<uploader>.Pop() ?? new uploader();
                uploader.permission = permission;
                uploader.listName = listName;
                uploader.onUpload = onUpload;
                uploader.fileStream = fileStream;
                uploader.backupIdentity = backupIdentity;
                uploader.isTimeVersion = isTimeVersion;
                return uploader.uploadHandle;
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="listName">列表名称</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="isTimeVersion">是否匹配时间版本</param>
        /// <param name="onUpload">上传结束委托</param>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void upload(fastCSharp.net.tcp.commandServer.socket client, listName listName, Stream fileStream, int backupIdentity, bool isTimeVersion
            , Func<returnValue<fileState>, bool> onUpload)
        {
            fileState fileState = fileState.NoPermission;
            try
            {
                if (verify(client))
                {
                    FileInfo file = new FileInfo(listName.Name);
                    listName.Set(file.FullName);
                    permission permission = getPermission(client, listName.LowerName);
                    if (verify(permission, permissionType.Write))
                    {
                        DirectoryInfo directory = file.Directory;
                        if (!directory.Exists) directory.Create();
                        Action upload = uploader.GetUpload(permission, onUpload, listName, fileStream, backupIdentity, isTimeVersion);
                        onUpload = null;
                        fileStream = null;
                        fastCSharp.threading.threadPool.TinyPool.Start(upload);
                    }
                }
            }
            catch (Exception error)
            {
                fileState = fileState.Exception;
                log.Error.Add(error, listName.Name, false);
            }
            pub.Dispose(ref fileStream);
            if (onUpload != null) onUpload(fileState);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="listName">列表名称</param>
        /// <param name="data">文件数据</param>
        /// <param name="isTimeVersion">是否匹配时间版本</param>
        /// <returns>文件上传状态</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private fileState upload(fastCSharp.net.tcp.commandServer.socket client, listName listName, byte[] data, int backupIdentity, bool isTimeVersion)
        {
            fileState fileState = fileState.NoPermission;
            try
            {
                if (verify(client))
                {
                    FileInfo file = new FileInfo(listName.Name);
                    listName.Set(file.FullName);
                    permission permission = getPermission(client, listName.LowerName);
                    if (verify(permission, permissionType.Write))
                    {
                        bool isFile = file.Exists;
                        if (isFile && isTimeVersion && file.LastWriteTimeUtc > listName.LastWriteTime)
                        {
                            return server.fileState.TimeVersionError;
                        }
                        if (!isFile || file.LastWriteTimeUtc != listName.LastWriteTime || listName.Length != file.Length)
                        {
                            if (isFile) permission.Backup(file, backupIdentity);
                            File.WriteAllBytes(listName.Name, data);
                            new FileInfo(listName.Name).LastWriteTimeUtc = listName.LastWriteTime;
                        }
                        fileState = fileState.Success;
                    }
                }
            }
            catch (Exception error)
            {
                fileState = fileState.Exception;
                log.Error.Add(error, listName.Name, false);
            }
            return fileState;
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="path">目录名称</param>
        /// <returns>是否成功</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private fileState createDirectory(fastCSharp.net.tcp.commandServer.socket client, string path)
        {
            fileState fileState = fileState.NoPermission;
            try
            {
                if (verify(client))
                {
                    DirectoryInfo directory = new DirectoryInfo(path);
                    if (verify(client, path = directory.fullName().toLower(), permissionType.Write))
                    {
                        if (directory.Exists) return fileState.Success;
                        directory.Create();
                        return fileState.FileNotFound;
                    }
                }
            }
            catch (Exception error)
            {
                fileState = fileState.Exception;
                log.Error.Add(error, path, false);
            }
            return fileState;
        }
        /// <summary>
        /// 文件下载器
        /// </summary>
        private sealed class downloader
        {
            /// <summary>
            /// 传输结束委托
            /// </summary>
            private Func<returnValue<listName>, bool> onDownload;
            /// <summary>
            /// 客户端文件流
            /// </summary>
            private Stream fileStream;
            /// <summary>
            /// 列表名称
            /// </summary>
            private listName listName;
            /// <summary>
            /// 数据缓冲区
            /// </summary>
            private byte[] buffer;
            /// <summary>
            /// 下载文件
            /// </summary>
            private Action downloadHandle;
            /// <summary>
            /// 文件传输器
            /// </summary>
            private downloader()
            {
                downloadHandle = download;
                buffer = new byte[fastCSharp.config.tcpCommand.Default.BigBufferSize];
            }
            /// <summary>
            /// 下载文件
            /// </summary>
            private void download()
            {
                listName listNameState = new listName { Length = (long)(byte)fileState.Exception, LastWriteTime = DateTime.MinValue };
                FileStream readStream = null;
                try
                {
                    FileInfo fileInfo = new FileInfo(listName.Name);
                    if (fileInfo.LastWriteTimeUtc != listName.LastWriteTime || fileInfo.Length != listName.Length)
                    {
                        listNameState.Set(string.Empty, fileInfo.LastWriteTimeUtc, fileInfo.Length);
                    }
                    else
                    {
                        long position = fileStream.Position;
                        readStream = new FileStream(listName.Name, FileMode.Open, FileAccess.Read, FileShare.Read);
                        long length = readStream.Length;
                        if (position < length)
                        {
                            if (position != 0) readStream.Seek(position, SeekOrigin.Begin);
                            while (position < length)
                            {
                                int read = readStream.Read(buffer, 0, buffer.Length);
                                if (read > 0)
                                {
                                    fileStream.Write(buffer, 0, read);
                                    position += read;
                                }
                                else break;
                            }
                        }
                        if (position == length) listNameState.Set(fileInfo.Name, fileInfo.LastWriteTimeUtc, length);
                        else listNameState.Length = (long)(byte)fileState.LengthError;
                    }
                }
                catch (Exception error)
                {
                    listNameState.Length = (long)(byte)fileState.Exception;
                    log.Error.Add(error, null, false);
                }
                Exception exception = null;
                pub.Dispose(ref readStream, ref exception);
                if (exception != null)
                {
                    log.Error.Add(exception, null, false);
                    if (listNameState.Name != null)
                    {
                        listNameState.Length = (long)(byte)fileState.Exception;
                        listNameState.Set(null);
                    }
                }
                push(listNameState);
            }
            /// <summary>
            /// 文件操作结束
            /// </summary>
            /// <param name="listName">列表名称</param>
            private void push(listName listName)
            {
                Func<returnValue<listName>, bool> onDownload = this.onDownload;
                this.onDownload = null;
                pub.Dispose(ref fileStream);
                typePool<downloader>.PushNotNull(this);
                onDownload(listName);
            }
            /// <summary>
            /// 获取文件下载器
            /// </summary>
            /// <param name="onDownload">下载结束委托</param>
            /// <param name="listName">列表名称</param>
            /// <param name="fileStream">文件流</param>
            /// <returns>文件下载器</returns>
            public static Action GetDownload
                (Func<returnValue<listName>, bool> onDownload, listName listName, Stream fileStream)
            {
                downloader downloader = typePool<downloader>.Pop() ?? new downloader();
                downloader.listName = listName;
                downloader.onDownload = onDownload;
                downloader.fileStream = fileStream;
                return downloader.downloadHandle;
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="listName">列表名称</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="onDownload">下载结束委托</param>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void download(fastCSharp.net.tcp.commandServer.socket client, listName listName, Stream fileStream
            , Func<returnValue<listName>, bool> onDownload)
        {
            listName listNameState = new listName { Length = (long)(byte)fileState.NoPermission, LastWriteTime = DateTime.MinValue };
            try
            {
                if (verify(client))
                {
                    FileInfo file = new FileInfo(listName.Name);
                    listName.Set(file.FullName);
                    if (verify(client, listName.LowerName, permissionType.Read))
                    {
                        if (file.Exists)
                        {
                            fastCSharp.threading.threadPool.TinyPool.Start(downloader.GetDownload(onDownload, listName, fileStream));
                            return;
                        }
                        listNameState.Length = (long)(byte)fileState.FileNotFound;
                    }
                }
            }
            catch (Exception error)
            {
                listNameState.Length = (long)(byte)fileState.Exception;
                log.Error.Add(error, null, false);
            }
            pub.Dispose(ref fileStream);
            onDownload(listNameState);
        }
        /// <summary>
        /// 客户端标识验证
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <returns>是否验证成功</returns>
        private static bool verify(fastCSharp.net.tcp.commandServer.socket client)
        {
            user user = (user)client.ClientUserInfo;
            if (user != null)
            {
                user cacheUser = user.Table.Cache.Get(user.Name);
                if (cacheUser != null && user.Password == cacheUser.Password) return true;
            }
            return false;
        }
        /// <summary>
        /// 获取文件权限
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="fileName">文件名</param>
        /// <param name="permissionType">文件权限</param>
        /// <returns>是否成功</returns>
        private unsafe static bool verify(fastCSharp.net.tcp.commandServer.socket client, string fileName, permissionType permissionType)
        {
            return (getPermissionType(client, fileName) & permissionType) != permissionType.None;
        }
        /// <summary>
        /// 获取文件权限
        /// </summary>
        /// <param name="permission">文件权限</param>
        /// <param name="permissionType">文件权限</param>
        /// <returns>是否成功</returns>
        private unsafe static bool verify(permission permission, permissionType permissionType)
        {
            return permission != null && (permission.Type & permissionType) != permissionType.None;
        }
        /// <summary>
        /// 获取文件权限
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件权限,失败返回null</returns>
        private unsafe static permission getPermission(fastCSharp.net.tcp.commandServer.socket client, string fileName)
        {
#if NotFastCSharpCode
            fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
            string userName = ((user)client.ClientUserInfo).Name;
            fixed (char* pathFixed = fileName)
            {
                char directorySeparatorChar = Path.DirectorySeparatorChar;
                for (char* read = pathFixed + fileName.Length; read != pathFixed; )
                {
                    if (*--read == directorySeparatorChar)
                    {
                        permission permission = permission.Table.Cache.Get(new permission.primaryKey { UserName = userName, Path = new string(pathFixed, 0, (int)(read - pathFixed) + 1) });
                        if (permission != null) return permission;
                    }
                }
            }
#endif
            return null;
        }
        /// <summary>
        /// 获取文件权限
        /// </summary>
        /// <param name="client">客户端标识</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件权限</returns>
        private unsafe static permissionType getPermissionType(fastCSharp.net.tcp.commandServer.socket client, string fileName)
        {
            permission permission = getPermission(client, fileName);
            return permission != null ? permission.Type : permissionType.None;
        }
        /// <summary>
        /// MD5
        /// </summary>
        private static readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        /// <summary>
        /// MD5访问锁
        /// </summary>
        private static readonly object md5Lock = new object();
        /// <summary>
        /// MD5缓冲区
        /// </summary>
        private static byte[] md5Buffer = new byte[32 + 256];
        /// <summary>
        /// 获取MD5密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="verifyTime">验证时间</param>
        /// <returns>MD5密码</returns>
        public unsafe static byte[] Md5Password(string password, DateTime verifyTime)
        {
            fixed (byte* md5Fixed = md5Buffer)
            fixed (char* passwordFixed = password)
            {
                ulong ticks = (ulong)verifyTime.Ticks;
                int copyLength = password.Length <= 128 ? (password.Length << 1) : 256;
                Monitor.Enter(md5Lock);
                try
                {
                    ((uint)ticks).UnsafeToHex((char*)(md5Fixed + 16));
                    ((uint)(ticks >> 32)).UnsafeToHex((char*)md5Fixed);
                    unsafer.memory.Copy(passwordFixed, md5Fixed + 32, copyLength);
                    return md5.ComputeHash(md5Buffer, 0, copyLength + 32);
                }
                finally { Monitor.Exit(md5Lock); }
            }
        }
    }
}
