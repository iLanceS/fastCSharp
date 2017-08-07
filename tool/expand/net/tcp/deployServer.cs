using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// 部署服务
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpServer(Service = deployServer.ServiceName, IsIdentityCommand = true, IsCompress = true)]
#else
    [fastCSharp.code.cSharp.tcpServer(Service = deployServer.ServiceName, IsIdentityCommand = true, IsCompress = true, VerifyMethodType = typeof(deployServer.tcpClient.timeVerifyMethod))]
#endif
    public partial class deployServer : timeVerifyServer
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        internal const string ServiceName = "deploy";
        /// <summary>
        /// 部署信息
        /// </summary>
        private struct deployInfo
        {
            /// <summary>
            /// 部署信息索引编号
            /// </summary>
            public int Identity;
            /// <summary>
            /// 文件数据源
            /// </summary>
            public byte[][] Files;
            /// <summary>
            /// 启动的定时任务
            /// </summary>
            public timer Timer;
            /// <summary>
            /// 任务集合
            /// </summary>
            public subArray<task> Tasks;
            /// <summary>
            /// 清除部署信息
            /// </summary>
            public void Clear()
            {
                Files = null;
                Array.Clear(Tasks.UnsafeArray, 0, Tasks.Count);
                Tasks.Empty();
                if (Timer != null)
                {
                    Timer.IsCancel = true;
                    Timer = null;
                }
                ++Identity;
            }
            /// <summary>
            /// 清除部署信息
            /// </summary>
            /// <param name="identity"></param>
            /// <returns></returns>
            public bool Clear(int identity)
            {
                if (Identity == identity)
                {
                    Clear();
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 设置文件数据源
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="files"></param>
            /// <returns></returns>
            public bool SetFiles(int identity, byte[][] files)
            {
                if (Identity == identity)
                {
                    Files = files;
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 添加任务
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="task"></param>
            /// <returns>任务索引编号,-1表示失败</returns>
            public int AddTask(int identity, task task)
            {
                if (Identity == identity)
                {
                    int index = Tasks.Count;
                    Tasks.Add(task);
                    return index;
                }
                return -1;
            }
            /// <summary>
            /// 启动部署
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="time"></param>
            /// <param name="timer"></param>
            /// <returns></returns>
            public bool Start(ref indexIdentity identity, DateTime time, timer timer)
            {
                if (Identity == identity.Identity && Tasks.Count != 0 && Timer == null)
                {
                    (Timer = timer).DeployInfo = this;
                    fastCSharp.threading.timerTask.Default.Add(timer.Start, time);
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 定时任务
        /// </summary>
        private sealed class timer
        {
            /// <summary>
            /// 部署信息索引标识
            /// </summary>
            public indexIdentity Identity;
            /// <summary>
            /// 部署服务
            /// </summary>
            public deployServer Server;
            /// <summary>
            /// 错误异常
            /// </summary>
            public Exception Error;
            /// <summary>
            /// 部署信息
            /// </summary>
            public deployInfo DeployInfo;
            /// <summary>
            /// 备份目录
            /// </summary>
            public DirectoryInfo BakDirectory;
            /// <summary>
            /// 当前任务标识
            /// </summary>
            public int TaskIndex;
            /// <summary>
            /// 是否已经取消定时任务
            /// </summary>
            public bool IsCancel;
            /// <summary>
            /// 启动定时任务
            /// </summary>
            public void Start()
            {
                if (!IsCancel)
                {
                    try
                    {
                        (BakDirectory = new DirectoryInfo(date.Now.ToString("yyyyMMddHHmmss_" + Identity.Index.toString() + "_" + Identity.Identity.toString()))).Create();
                        while (TaskIndex != DeployInfo.Tasks.Count && !IsCancel)
                        {
                            DeployInfo.Tasks.UnsafeArray[TaskIndex].Run(this);
                            ++TaskIndex;
                        }
                    }
                    catch (Exception error)
                    {
                        Error = error;
                    }
                    finally
                    {
                        if (Error == null) Server.clear(Identity);
                        else log.Error.Add(Error, null, false);
                    }
                }
            }
            /// <summary>
            /// 创建备份目录
            /// </summary>
            /// <returns></returns>
            internal DirectoryInfo CreateBakDirectory()
            {
                DirectoryInfo bakDirectory = new DirectoryInfo(BakDirectory.fullName() + TaskIndex.toString());
                bakDirectory.Create();
                return bakDirectory;
            }
        }
        /// <summary>
        /// 任务信息
        /// </summary>
        private sealed class task
        {
            /// <summary>
            /// 服务器端目录
            /// </summary>
            public DirectoryInfo ServerDirectory;
            /// <summary>
            /// 目录信息
            /// </summary>
            public directory Directory;
            /// <summary>
            /// 文件集合
            /// </summary>
            public keyValue<string, int>[] FileIndexs;
            /// <summary>
            /// 任务信息索引位置
            /// </summary>
            public int TaskIndex;
            /// <summary>
            /// 运行前休眠
            /// </summary>
            public int RunSleep;
            /// <summary>
            /// 是否执行other目录文件
            /// </summary>
            public bool IsRunOther;
            /// <summary>
            /// 任务类型
            /// </summary>
            public taskType Type;
            /// <summary>
            /// 运行任务
            /// </summary>
            /// <param name="timer"></param>
            public void Run(timer timer)
            {
                switch (Type)
                {
                    case taskType.Run: run(timer); break;
                    case taskType.Web: if (Directory.Name != null) Directory.Deploy(ServerDirectory, timer.CreateBakDirectory()); break;
                    case taskType.File: file(timer); break;
                    case taskType.WaitRunSwitch: wait(timer); break;
                }
            }
            /// <summary>
            /// 判断文件是否可写
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            private bool canWrite(string file)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Exists)
                {
                    try
                    {
                        using (FileStream fileStream = fileInfo.OpenWrite()) return true;
                    }
                    catch { }
                    return false;
                }
                return true;
            }
            /// <summary>
            /// 写文件并运行程序
            /// </summary>
            /// <param name="timer"></param>
            private void run(timer timer)
            {
                string serverDirectoryName = ServerDirectory.fullName(), runFileName = serverDirectoryName + FileIndexs[0].Key;
                DirectoryInfo otherServerDirectory = new DirectoryInfo(serverDirectoryName + "other");
                if (otherServerDirectory.Exists && !canWrite(runFileName))
                {
                    ServerDirectory = otherServerDirectory;
                    runFileName = otherServerDirectory.fullName() + FileIndexs[0].Key;
                    IsRunOther = true;
                }
                file(timer);
                Thread.Sleep(RunSleep);
                fastCSharp.diagnostics.process.StartDirectory(runFileName, null);
            }
            /// <summary>
            /// 写文件
            /// </summary>
            /// <param name="timer"></param>
            private void file(timer timer)
            {
                string serverDirectoryName = ServerDirectory.fullName(), bakDirectoryName = timer.CreateBakDirectory().fullName();
                foreach (keyValue<string, int> fileIndex in FileIndexs)
                {
                    byte[] data = timer.DeployInfo.Files[fileIndex.Value];
                    string fileName = serverDirectoryName + fileIndex.Key;
                    FileInfo file = new FileInfo(fileName);
                    if (file.Exists) File.Move(fileName, bakDirectoryName + fileIndex.Key);
                    using (FileStream fileStream = file.Create()) fileStream.Write(data, 0, data.Length);
                }
            }
            /// <summary>
            /// 等待运行程序切换结束的文件
            /// </summary>
            private string waitFile
            {
                get
                {
                    if (IsRunOther) return ServerDirectory.Parent.fullName() + FileIndexs[0].Key;
                    return ServerDirectory.fullName() + "other" + fastCSharp.directory.DirectorySeparator + FileIndexs[0].Key;
                }
            }
            /// <summary>
            /// 等待运行程序切换结束
            /// </summary>
            /// <param name="timer"></param>
            private void wait(timer timer)
            {
                FileInfo file = new FileInfo(timer.DeployInfo.Tasks.UnsafeArray[TaskIndex].waitFile);
                if (file.Exists)
                {
                    do
                    {
                        try
                        {
                            using (FileStream fileStream = file.OpenWrite()) return;
                        }
                        catch { }
                        Thread.Sleep(1);
                    }
                    while (true);
                }
            }
        }
        /// <summary>
        /// 任务类型
        /// </summary>
        internal enum taskType : byte
        {
            /// <summary>
            /// 写文件并运行程序
            /// </summary>
            Run,
            /// <summary>
            /// css/js/html
            /// </summary>
            Web,
            /// <summary>
            /// 写文件
            /// </summary>
            File,
            /// <summary>
            /// 等待运行程序切换结束
            /// </summary>
            WaitRunSwitch
        }
        /// <summary>
        /// 文件最后修改时间
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
        public struct fileTime
        {
            /// <summary>
            /// 文件最后修改时间
            /// </summary>
            public DateTime LastWriteTimeUtc;
            /// <summary>
            /// 文件名称
            /// </summary>
            public string FileName;
            /// <summary>
            /// 文件数据
            /// </summary>
            public byte[] Data;
            /// <summary>
            /// 文件最后修改时间
            /// </summary>
            /// <param name="file"></param>
            internal fileTime(FileInfo file)
            {
                LastWriteTimeUtc = file.LastWriteTimeUtc;
                FileName = file.Name;
                Data = null;
            }
            /// <summary>
            /// 设置文件最后修改时间
            /// </summary>
            /// <param name="time"></param>
            /// <param name="fileName"></param>
            internal void Set(DateTime time, string fileName)
            {
                LastWriteTimeUtc = time;
                FileName = fileName;
            }
            /// <summary>
            /// 设置文件最后修改时间
            /// </summary>
            /// <param name="fileName"></param>
            internal void Set(string fileName)
            {
                LastWriteTimeUtc = DateTime.MinValue;
                FileName = fileName;
            }
            /// <summary>
            /// 加载文件数据
            /// </summary>
            /// <param name="path"></param>
            internal void Load(string path)
            {
                FileInfo file = new FileInfo(path + FileName);
                if (file.LastWriteTimeUtc >= LastWriteTimeUtc)
                {
                    Data = File.ReadAllBytes(file.FullName);
                    LastWriteTimeUtc = file.LastWriteTimeUtc;
                }
                else log.Default.Add("文件同步时间冲突 " + file.FullName, new System.Diagnostics.StackFrame(), false);
            }
        }
        /// <summary>
        /// 目录信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
        public struct directory
        {
            /// <summary>
            /// 目录名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 目录信息集合
            /// </summary>
            public directory[] Directorys;
            /// <summary>
            /// 文件最后修改时间集合
            /// </summary>
            public fileTime[] Files;
            /// <summary>
            /// 服务器端比较文件最后修改时间
            /// </summary>
            /// <param name="directory"></param>
            internal void Different(DirectoryInfo directoryInfo)
            {
                string directoryName = directoryInfo.fullName();
                if (Directorys != null)
                {
                    int directoryCount = 0;
                    foreach (directory directory in Directorys)
                    {
                        DirectoryInfo nextDirectoryInfo = new DirectoryInfo(directoryName + directory.Name);
                        if (nextDirectoryInfo.Exists)
                        {
                            directory.Different(nextDirectoryInfo);
                            if (directory.Name != null) Directorys[directoryCount++] = directory;
                        }
                        else Directorys[directoryCount++] = directory;
                    }
                    if (directoryCount == 0) Directorys = null;
                    else if (directoryCount != Directorys.Length) Array.Resize(ref Directorys, directoryCount);
                }
                if (Files != null)
                {
                    int fileCount = 0;
                    foreach (fileTime fileTime in Files)
                    {
                        FileInfo file = new FileInfo(directoryName + fileTime.FileName);
                        if (file.Exists)
                        {
                            if (file.LastWriteTimeUtc != fileTime.LastWriteTimeUtc) Files[fileCount++].Set(file.LastWriteTimeUtc, fileTime.FileName);
                        }
                        else Files[fileCount++].Set(fileTime.FileName);
                    }
                    if (fileCount == 0) Files = null;
                    else if (fileCount != Files.Length) Array.Resize(ref Files, fileCount);
                }
                if (Files == null && Directorys == null) Name = null;
            }
            /// <summary>
            /// 加载文件数据
            /// </summary>
            /// <param name="directoryInfo"></param>
            public void Load(DirectoryInfo directoryInfo)
            {
                string directoryName = directoryInfo.fullName();
                if (Directorys != null)
                {
                    for (int index = Directorys.Length; index != 0; Directorys[--index].load(directoryName)) ;
                }
                if (Files != null)
                {
                    for (int index = Files.Length; index != 0; Files[--index].Load(directoryName)) ;
                }
            }
            /// <summary>
            /// 加载文件数据
            /// </summary>
            /// <param name="path"></param>
            private void load(string path)
            {
                Load(new DirectoryInfo(path + Name));
            }
            /// <summary>
            /// 服务器端部署
            /// </summary>
            /// <param name="serverDirectory"></param>
            /// <param name="bakDirectory"></param>
            internal void Deploy(DirectoryInfo serverDirectory, DirectoryInfo bakDirectory)
            {
                string serverDirectoryName = serverDirectory.fullName(), bakDirectoryName = bakDirectory.fullName();
                if (Directorys != null)
                {
                    foreach (directory directory in Directorys)
                    {
                        DirectoryInfo nextServerDirectory = new DirectoryInfo(serverDirectoryName + directory.Name), nextBakDirectory = new DirectoryInfo(bakDirectoryName + directory.Name);
                        if (!nextServerDirectory.Exists) nextServerDirectory.Create();
                        if (!nextBakDirectory.Exists) nextBakDirectory.Create();
                        directory.Deploy(nextServerDirectory, nextBakDirectory);
                    }
                }
                if (Files != null)
                {
                    foreach (fileTime fileTime in Files)
                    {
                        if (fileTime.Data != null)
                        {
                            string fileName = serverDirectoryName + fileTime.FileName;
                            FileInfo file = new FileInfo(fileName);
                            if (file.Exists)
                            {
                                string newFileName = serverDirectoryName + "%" + fileTime.FileName;
                                using (FileStream fileStream = new FileStream(newFileName, FileMode.Create)) fileStream.Write(fileTime.Data, 0, fileTime.Data.Length);
                                new FileInfo(newFileName).LastWriteTimeUtc = fileTime.LastWriteTimeUtc;
                                File.Move(fileName, bakDirectoryName + fileTime.FileName);
                                File.Move(newFileName, fileName);
                            }
                            else
                            {
                                using (FileStream fileStream = file.Create()) fileStream.Write(fileTime.Data, 0, fileTime.Data.Length);
                                new FileInfo(fileName).LastWriteTimeUtc = fileTime.LastWriteTimeUtc;
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// 客户端创建目录信息
            /// </summary>
            /// <param name="directory"></param>
            /// <param name="lastWriteTime"></param>
            /// <returns></returns>
            public static directory CreateWeb(DirectoryInfo directory, DateTime lastWriteTime)
            {
                directory cssDirectory = page(directory, true, "css", null, lastWriteTime), jsDirectory = js(directory, true, lastWriteTime), loadJsDirectory = loadJs(directory, lastWriteTime), htmlDirectory = page(directory, true, "html", "page.html", lastWriteTime), imageDirectory = image(directory, lastWriteTime);
                cssDirectory.Files = array.concat(cssDirectory.Files, jsDirectory.Files, htmlDirectory.Files, imageDirectory.Files);
                if (cssDirectory.Files.Length == 0) cssDirectory.Files = null;
                cssDirectory.Directorys = array.concat(cssDirectory.Directorys, jsDirectory.Directorys, loadJsDirectory.Directorys, htmlDirectory.Directorys, imageDirectory.Directorys);
                if (cssDirectory.Directorys.Length == 0) cssDirectory.Directorys = null;
                cssDirectory.Name = string.Empty;
                return cssDirectory;
            }
            /// <summary>
            /// 创建文件目录信息
            /// </summary>
            /// <param name="directoryInfo"></param>
            /// <param name="isBoot"></param>
            /// <param name="extension"></param>
            /// <param name="pageExtension"></param>
            /// <param name="lastWriteTime"></param>
            /// <returns></returns>
            private static directory page(DirectoryInfo directoryInfo, bool isBoot, string extension, string pageExtension, DateTime lastWriteTime)
            {
                directory directory = new directory();
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                if (directoryInfos.Length != 0)
                {
                    subArray<directory> directorys = new subArray<directory>(directoryInfos.Length);
                    foreach (DirectoryInfo nextDirectoryInfo in directoryInfos)
                    {
                        if (!isBoot || nextDirectoryInfo.Name != "js")
                        {
                            directory nextDirectory = page(nextDirectoryInfo, false, extension, pageExtension, lastWriteTime);
                            if (nextDirectory.Name != null) directorys.Add(nextDirectory);
                        }
                    }
                    if (directorys.Count != 0) directory.Directorys = directorys.ToArray();
                }
                FileInfo[] files = directoryInfo.GetFiles("*." + extension);
                if (files.Length != 0)
                {
                    subArray<fileTime> fileTimes = new subArray<fileTime>(files.Length);
                    foreach (FileInfo file in files)
                    {
                        if (file.LastWriteTimeUtc > lastWriteTime && (pageExtension == null || !file.Name.EndsWith(pageExtension, StringComparison.Ordinal))) fileTimes.Add(new fileTime(file));
                    }
                    if (fileTimes.Count != 0) directory.Files = fileTimes.ToArray();
                }
                if (directory.Files != null || directory.Directorys != null) directory.Name = directoryInfo.Name;
                return directory;
            }
            /// <summary>
            /// 创建JS脚本文件目录信息
            /// </summary>
            /// <param name="directoryInfo"></param>
            /// <param name="isBoot"></param>
            /// <param name="lastWriteTime"></param>
            /// <returns></returns>
            private static directory js(DirectoryInfo directoryInfo, bool isBoot, DateTime lastWriteTime)
            {
                directory directory = new directory();
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                if (directoryInfos.Length != 0)
                {
                    subArray<directory> directorys = new subArray<directory>(directoryInfos.Length);
                    foreach (DirectoryInfo nextDirectoryInfo in directoryInfos)
                    {
                        if (!isBoot || nextDirectoryInfo.Name != "js")
                        {
                            directory nextDirectory = js(nextDirectoryInfo, false, lastWriteTime);
                            if (nextDirectory.Name != null) directorys.Add(nextDirectory);
                        }
                    }
                    if (directorys.Count != 0) directory.Directorys = directorys.ToArray();
                }
                FileInfo[] files = directoryInfo.GetFiles("*.js");
                if (files.Length != 0)
                {
                    subArray<fileTime> fileTimes = new subArray<fileTime>(files.Length);
                    foreach (FileInfo file in files)
                    {
                        if (file.LastWriteTimeUtc > lastWriteTime && !file.Name.EndsWith(".page.js", StringComparison.Ordinal)) fileTimes.Add(new fileTime(file));
                    }
                    if (fileTimes.Count != 0) directory.Files = fileTimes.ToArray();
                }
                if (directory.Files != null || directory.Directorys != null) directory.Name = directoryInfo.Name;
                return directory;
            }
            /// <summary>
            /// 创建JS脚本文件类库目录信息
            /// </summary>
            /// <param name="directoryInfo"></param>
            /// <param name="lastWriteTime"></param>
            /// <returns></returns>
            private static directory loadJs(DirectoryInfo directoryInfo, DateTime lastWriteTime)
            {
                DirectoryInfo jsDirectory = new DirectoryInfo(directoryInfo.fullName() + "js");
                directory directory = new directory();
                if (jsDirectory.Exists)
                {
                    DirectoryInfo[] directoryInfos = jsDirectory.GetDirectories();
                    if (directoryInfos.Length != 0)
                    {
                        subArray<directory> directorys = new subArray<directory>(directoryInfos.Length);
                        foreach (DirectoryInfo nextDirectoryInfo in directoryInfos)
                        {
                            directory nextDirectory;
                            switch (nextDirectoryInfo.Name)
                            {
                                case "ace": nextDirectory = js(nextDirectoryInfo, lastWriteTime, new string[] { "ace.js" }); break;
                                case "mathJax": nextDirectory = js(nextDirectoryInfo, lastWriteTime, new string[] { "MathJax.js" }); break;
                                case "highcharts": nextDirectory = js(nextDirectoryInfo, false, lastWriteTime); break;
                                default: fastCSharp.log.Error.Throw("未知的js文件夹 " + nextDirectoryInfo.fullName(), new System.Diagnostics.StackFrame(), false); nextDirectory = new directory(); break;
                            }
                            if (nextDirectory.Name != null) directorys.Add(nextDirectory);
                        }
                        if (directorys.Count != 0) directory.Directorys = directorys.ToArray();
                    }
                    FileInfo[] files = jsDirectory.GetFiles("*.js");
                    if (files.Length != 0)
                    {
                        subArray<fileTime> fileTimes = new subArray<fileTime>(files.Length);
                        fileTime loadFileTime = new fileTime(), loadPageFileTime = new fileTime();
                        foreach (FileInfo file in files)
                        {
                            if (file.LastWriteTimeUtc > lastWriteTime)
                            {
                                if (file.Name == "load.js") loadFileTime = new fileTime(file);
                                else if (file.Name == "loadPage.js") loadPageFileTime = new fileTime(file);
                                else fileTimes.Add(new fileTime(file));
                            }
                        }
                        if (loadFileTime.FileName != null) fileTimes.Add(loadFileTime);
                        if (loadPageFileTime.FileName != null) fileTimes.Add(loadPageFileTime);
                        if (fileTimes.Count != 0) directory.Files = fileTimes.ToArray();
                    }
                    if (directory.Files != null || directory.Directorys != null)
                    {
                        directory.Name = jsDirectory.Name;
                        directory = new directory { Name = directoryInfo.Name, Directorys = new directory[] { directory } };
                    }
                }
                return directory;
            }
            /// <summary>
            /// 创建JS脚本文件目录信息
            /// </summary>
            /// <param name="directoryInfo"></param>
            /// <param name="lastWriteTime"></param>
            /// <param name="fileNames"></param>
            /// <returns></returns>
            private static directory js(DirectoryInfo directoryInfo, DateTime lastWriteTime, string[] fileNames)
            {
                directory directory = new directory();
                subArray<fileTime> fileTimes = new subArray<fileTime>(fileNames.Length);
                string path = directoryInfo.fullName();
                foreach (string fileName in fileNames)
                {
                    FileInfo file = new FileInfo(path + fileName);
                    if (file.Exists && file.LastWriteTimeUtc > lastWriteTime) fileTimes.Add(new fileTime(file));
                }
                if (fileTimes.Count != 0)
                {
                    directory.Files = fileTimes.ToArray();
                    directory.Name = directoryInfo.Name;
                }
                return directory;
            }
            /// <summary>
            /// 图片目录信息
            /// </summary>
            /// <param name="directoryInfo"></param>
            /// <param name="lastWriteTime"></param>
            /// <returns></returns>
            private static directory image(DirectoryInfo directoryInfo, DateTime lastWriteTime)
            {
                DirectoryInfo imageDirectory = new DirectoryInfo(directoryInfo.fullName() + "images");
                directory directory = new directory();
                if (imageDirectory.Exists)
                {
                    directory = page(imageDirectory, false, "*.*", null, lastWriteTime);
                    if (directory.Name != null) directory = new directory { Name = directoryInfo.Name, Directorys = new directory[] { directory } };
                }
                return directory;
            }
        }
        /// <summary>
        /// 部署服务客户端
        /// </summary>
        public sealed class client
        {
            /// <summary>
            /// 客户端部署信息
            /// </summary>
            public struct deployInfo
            {
                /// <summary>
                /// 部署名称
                /// </summary>
                public string Name;
#pragma warning disable 649
                /// <summary>
                /// 任务信息
                /// </summary>
                internal task[] Tasks;
#pragma warning restore 649
                /// <summary>
                /// 部署服务端口信息
                /// </summary>
                public host Host;
                /// <summary>
                /// 验证字符串
                /// </summary>
                public string VerifyString;
                /// <summary>
                /// 部署
                /// </summary>
                /// <param name="client"></param>
                /// <returns></returns>
                public bool Deploy(client client)
                {
#if NotFastCSharpCode
                    fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.NotFastCSharpCode);
#else
                    if (Tasks.length() != 0)
                    {
                        fastCSharp.code.cSharp.tcpServer attribute = fastCSharp.code.cSharp.tcpServer.GetConfig("deploy", typeof(fastCSharp.net.tcp.deployServer));
                        if (Host.Host != null) attribute.Host = Host.Host;
                        if (Host.Port != 0) attribute.Port = Host.Port;
                        if (VerifyString != null) attribute.VerifyString = VerifyString;
                        using (tcpClient tcpClient = new tcpClient(attribute))
                        {
                            indexIdentity identity = tcpClient.create();
                            byte isStart = 0;
                            try
                            {
                                Dictionary<string, fileSource> fileSources = dictionary.CreateOnly<string, fileSource>();
                                taskInfo[] tasks = new taskInfo[Tasks.Length];
                                for (int taskIndex = 0; taskIndex != Tasks.Length; ++taskIndex)
                                {
                                    switch (Tasks[taskIndex].Type)
                                    {
                                        case taskType.Run:
                                        case taskType.File: appendSource(client, fileSources, ref Tasks[taskIndex], ref tasks[taskIndex]); break;
                                        case taskType.Web:
                                            DirectoryInfo clientDirectory = new DirectoryInfo(Tasks[taskIndex].ClientPath);
                                            directory directory = directory.CreateWeb(clientDirectory, client.FileLastWriteTime);
                                            tasks[taskIndex].Directory = tcpClient.getFileDifferent(directory, Tasks[taskIndex].ServerPath);
                                            tasks[taskIndex].Directory.Load(clientDirectory);
                                            break;
                                    }
                                }

                                if (fileSources.Count != 0 && !tcpClient.setFileSource(identity, fileSources.getArray(value => value.Value.Data))) return false;
                                for (int taskIndex = 0; taskIndex != Tasks.Length; ++taskIndex)
                                {
                                    switch (Tasks[taskIndex].Type)
                                    {
                                        case taskType.Run:
                                            if ((tasks[taskIndex].TaskIndex = tcpClient.addRun(identity, tasks[taskIndex].FileIndexs.ToArray(), Tasks[taskIndex].ServerPath, Tasks[taskIndex].RunSleep)) == -1) return false;
                                            break;
                                        case taskType.Web:
                                            if (tcpClient.addWeb(identity, tasks[taskIndex].Directory, Tasks[taskIndex].ServerPath) == -1) return false;
                                            break;
                                        case taskType.File:
                                            if (tcpClient.addFiles(identity, tasks[taskIndex].FileIndexs.ToArray(), Tasks[taskIndex].ServerPath) == -1) return false;
                                            break;
                                        case taskType.WaitRunSwitch:
                                            if (tcpClient.addWaitRunSwitch(identity, tasks[Tasks[taskIndex].TaskIndex].TaskIndex) == -1) return false;
                                            break;
                                    }
                                }
                                if (tcpClient.start(identity, DateTime.MinValue))
                                {
                                    isStart = 1;
                                    return true;
                                }
                            }
                            finally
                            {
                                try
                                {
                                    if (isStart == 0) tcpClient.clear(identity);
                                }
                                catch { }
                            }
                        }
                    }
#endif
                    return false;
                }
                /// <summary>
                /// 添加文件数据源
                /// </summary>
                /// <param name="client"></param>
                /// <param name="fileSources"></param>
                /// <param name="task"></param>
                /// <param name="serverTask"></param>
                private void appendSource(client client, Dictionary<string, fileSource> fileSources, ref task task, ref taskInfo serverTask)
                {
                    DirectoryInfo directory = new DirectoryInfo(task.ClientPath);
                    string directoryName = directory.fullName();
                    if (task.RunFileName != null) appendSource(client, fileSources, directoryName, task.RunFileName, ref serverTask);
                    foreach (FileInfo file in directory.GetFiles("*.exe"))
                    {
                        if (file.LastWriteTimeUtc > client.FileLastWriteTime && file.Name != task.RunFileName && !client.ignoreFileNames.Contains(file.Name)
                            && !file.Name.EndsWith(".vshost.exe", StringComparison.Ordinal))
                        {
                            appendSource(client, fileSources, directoryName, file.Name, ref serverTask);
                        }
                    }
                    foreach (FileInfo file in directory.GetFiles("*.dll"))
                    {
                        if (file.LastWriteTimeUtc > client.FileLastWriteTime && !client.ignoreFileNames.Contains(file.Name))
                        {
                            appendSource(client, fileSources, directoryName, file.Name, ref serverTask);
                        }
                    }
                    foreach (FileInfo file in directory.GetFiles("*.pdb"))
                    {
                        if (file.LastWriteTimeUtc > client.FileLastWriteTime && !client.ignoreFileNames.Contains(file.Name))
                        {
                            appendSource(client, fileSources, directoryName, file.Name, ref serverTask);
                        }
                    }
                }
                /// <summary>
                /// 添加文件数据源
                /// </summary>
                /// <param name="fileSources"></param>
                /// <param name="path"></param>
                /// <param name="fileName"></param>
                /// <param name="serverTask"></param>
                private void appendSource(client client, Dictionary<string, fileSource> fileSources, string path, string fileName, ref taskInfo serverTask)
                {
                    fileSource fileSource;
                    if (!fileSources.TryGetValue(fileName, out fileSource))
                    {
                        foreach (string runFilePath in client.runFilePaths)
                        {
                            string runFileName = runFilePath + fileName;
                            if (File.Exists(runFileName))
                            {
                                fileSources.Add(fileName, fileSource = new fileSource { Data = File.ReadAllBytes(runFileName), Index = fileSources.Count });
                                break;
                            }
                        }
                        if (fileSource.Data == null) fileSources.Add(fileName, fileSource = new fileSource { Data = File.ReadAllBytes(path + fileName), Index = fileSources.Count });
                    }
                    serverTask.FileIndexs.Add(new keyValue<string, int>(fileName, fileSource.Index));
                }
                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    return Host.Host + ":" + Host.Port.toString() + ":" + Name;
                }
            }
#pragma warning disable 649
            /// <summary>
            /// 任务信息
            /// </summary>
            internal struct task
            {
                /// <summary>
                /// 服务器端目录
                /// </summary>
                public string ServerPath;
                /// <summary>
                /// 客户端目录
                /// </summary>
                public string ClientPath;
                /// <summary>
                /// 运行文件名称
                /// </summary>
                public string RunFileName;
                /// <summary>
                /// 任务信息索引位置
                /// </summary>
                public int TaskIndex;
                /// <summary>
                /// 运行前休眠
                /// </summary>
                public int RunSleep;
                /// <summary>
                /// 任务类型
                /// </summary>
                public taskType Type;
            }
#pragma warning restore 649
            /// <summary>
            /// 任务信息客户端扩展信息
            /// </summary>
            private struct taskInfo
            {
                /// <summary>
                /// 文件数据源索引集合
                /// </summary>
                public subArray<keyValue<string, int>> FileIndexs;
                /// <summary>
                /// Web部署目录信息
                /// </summary>
                public directory Directory;
                /// <summary>
                /// 服务器端任务索引
                /// </summary>
                public int TaskIndex;
            }
            /// <summary>
            /// 文件数据源
            /// </summary>
            private struct fileSource
            {
                /// <summary>
                /// 文件数据
                /// </summary>
                public byte[] Data;
                /// <summary>
                /// 文件数据源索引
                /// </summary>
                public int Index;
            }
            /// <summary>
            /// 文件更新时间
            /// </summary>
            public DateTime FileLastWriteTime;
            /// <summary>
            /// 客户端部署信息集合
            /// </summary>
            private deployInfo[] deploys;
            /// <summary>
            /// 客户端部署信息集合
            /// </summary>
            public deployInfo[] Deploys
            {
                get { return deploys; }
            }
            /// <summary>
            /// 文件匹配路径(用于过滤相同的文件名称)
            /// </summary>
            private string[] runFilePaths;
            /// <summary>
            /// 忽略文件名称
            /// </summary>
            private HashSet<string> ignoreFileNames;
            /// <summary>
            /// 部署服务客户端
            /// </summary>
            public client()
            {
                fastCSharp.config.pub.LoadConfig(this);
                if (deploys == null) deploys = nullValue<deployInfo>.Array;
                if (runFilePaths == null) runFilePaths = nullValue<string>.Array;
                else
                {
                    foreach (string runFilePath in runFilePaths)
                    {
                        if(!Directory.Exists(runFilePath))
                        {
                            log.Error.Add("没有找到文件匹配路径 " + runFilePath, new System.Diagnostics.StackFrame(), false);
                        }
                    }
                }
                if (ignoreFileNames == null) ignoreFileNames = hashSet.CreateOnly<string>();
            }
        }
        /// <summary>
        /// 部署信息池
        /// </summary>
        private indexValuePool<deployInfo> deployPool = new indexValuePool<deployInfo>(4);
        /// <summary>
        /// 清除所有部署任务
        /// </summary>
        [fastCSharp.code.cSharp.tcpMethod]
        private void clear()
        {
            if (deployPool.Enter())
            {
                deployInfo[] deployArray = deployPool.UnsafeArray;
                int poolIndex = deployPool.PoolIndex;
                while (poolIndex != 0) deployArray[--poolIndex].Clear();
                deployPool.ClearIndexContinue();
                deployPool.Exit();
            }
        }
        /// <summary>
        /// 创建部署
        /// </summary>
        /// <returns>部署信息索引标识</returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private indexIdentity create()
        {
            if (deployPool.Enter())
            {
                try
                {
                    int index = deployPool.GetIndexContinue();
                    return new net.indexIdentity { Index = index, Identity = deployPool.UnsafeArray[index].Identity };
                }
                finally { deployPool.Exit(); }
            }
            return new indexIdentity { Index = net.indexIdentity.ErrorIndex };
        }
        /// <summary>
        /// 清除部署信息
        /// </summary>
        /// <param name="identity">部署信息索引标识</param>
        [fastCSharp.code.cSharp.tcpMethod]
        private void clear(indexIdentity identity)
        {
            if (deployPool.Enter())
            {
                if ((uint)identity.Index < (uint)deployPool.PoolIndex && deployPool.UnsafeArray[identity.Index].Clear(identity.Identity)) deployPool.FreeExit(identity.Index);
                else deployPool.Exit();
            }
        }
        /// <summary>
        /// 启动部署
        /// </summary>
        /// <param name="identity">部署信息索引标识</param>
        /// <param name="time">启动时间</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false)]
        private bool start(indexIdentity identity, DateTime time)
        {
            if (deployPool.Enter())
            {
                if ((uint)identity.Index < (uint)deployPool.PoolIndex)
                {
                    try
                    {
                        if (deployPool.UnsafeArray[identity.Index].Start(ref identity, time, new timer { Server = this, Identity = identity })) return true;
                    }
                    finally { deployPool.Exit(); }
                    return false;
                }
                deployPool.Exit();
            }
            return false;
        }
        /// <summary>
        /// 设置文件数据源
        /// </summary>
        /// <param name="identity">部署信息索引标识</param>
        /// <param name="files">文件数据源</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private bool setFileSource(indexIdentity identity, byte[][] files)
        {
            if (deployPool.Enter())
            {
                if ((uint)identity.Index < (uint)deployPool.PoolIndex && deployPool.UnsafeArray[identity.Index].SetFiles(identity.Identity, files))
                {
                    deployPool.Exit();
                    return true;
                }
                deployPool.Exit();
            }
            return false;
        }
        /// <summary>
        /// 比较文件最后修改时间
        /// </summary>
        /// <param name="directory">目录信息</param>
        /// <param name="serverPath">服务器端路径</param>
        /// <returns></returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private directory getFileDifferent(directory directory, string serverPath)
        {
            directory.Different(new DirectoryInfo(serverPath));
            return directory;
        }
        /// <summary>
        /// 添加web任务(css/js/html)
        /// </summary>
        /// <param name="identity">部署信息索引标识</param>
        /// <param name="directory">目录信息</param>
        /// <param name="serverPath">服务器端路径</param>
        /// <returns>任务索引编号,-1表示失败</returns>
        [fastCSharp.code.cSharp.tcpMethod]
        private int addWeb(indexIdentity identity, directory directory, string serverPath)
        {
            if (deployPool.Enter())
            {
                if ((uint)identity.Index < (uint)deployPool.PoolIndex)
                {
                    try
                    {
                        return deployPool.UnsafeArray[identity.Index].AddTask(identity.Identity, new task { Directory = directory, ServerDirectory = new DirectoryInfo(serverPath), Type = taskType.Web });
                    }
                    finally { deployPool.Exit(); }
                }
                deployPool.Exit();
            }
            return -1;
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="identity">部署信息索引标识</param>
        /// <param name="files">文件集合</param>
        /// <param name="serverPath">服务器端路径</param>
        /// <returns>任务索引编号,-1表示失败</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false)]
        private int addFiles(indexIdentity identity, keyValue<string, int>[] files, string serverPath)
        {
            if (deployPool.Enter())
            {
                if ((uint)identity.Index < (uint)deployPool.PoolIndex)
                {
                    try
                    {
                        return deployPool.UnsafeArray[identity.Index].AddTask(identity.Identity, new task { FileIndexs = files, ServerDirectory = new DirectoryInfo(serverPath), Type = taskType.File });
                    }
                    finally { deployPool.Exit(); }
                }
                deployPool.Exit();
            }
            return -1;
        }
        /// <summary>
        /// 写文件并运行程序
        /// </summary>
        /// <param name="identity">部署信息索引标识</param>
        /// <param name="files">文件集合</param>
        /// <param name="serverPath">服务器端路径</param>
        /// <param name="runSleep">运行前休眠</param>
        /// <returns>任务索引编号,-1表示失败</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false)]
        private int addRun(indexIdentity identity, keyValue<string, int>[] files, string serverPath, int runSleep)
        {
            if (deployPool.Enter())
            {
                if ((uint)identity.Index < (uint)deployPool.PoolIndex)
                {
                    try
                    {
                        return deployPool.UnsafeArray[identity.Index].AddTask(identity.Identity, new task { FileIndexs = files, ServerDirectory = new DirectoryInfo(serverPath), Type = taskType.Run, RunSleep = runSleep });
                    }
                    finally { deployPool.Exit(); }
                }
                deployPool.Exit();
            }
            return -1;
        }
        /// <summary>
        /// 等待运行程序切换结束
        /// </summary>
        /// <param name="identity">部署信息索引标识</param>
        /// <param name="taskIndex">任务索引位置</param>
        /// <returns>任务索引编号,-1表示失败</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false)]
        private int addWaitRunSwitch(indexIdentity identity, int taskIndex)
        {
            if (deployPool.Enter())
            {
                if ((uint)identity.Index < (uint)deployPool.PoolIndex)
                {
                    try
                    {
                        return deployPool.UnsafeArray[identity.Index].AddTask(identity.Identity, new task { TaskIndex = taskIndex, Type = taskType.WaitRunSwitch });
                    }
                    finally { deployPool.Exit(); }
                }
                deployPool.Exit();
            }
            return -1;
        }
    }
}
