using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;

namespace fastCSharp.document
{
    /// <summary>
    /// 环境检测
    /// </summary>
    internal sealed class environment
    {
        /// <summary>
        /// 是否VS2010
        /// </summary>
#if VS2010
        public bool VS2010 = true;
#else
        public bool VS2010;
#endif
        /// <summary>
        /// fastCSharp项目路径
        /// </summary>
        public string FastCSharpPath;
        /// <summary>
        /// 环境检测结果
        /// </summary>
        public static readonly environment Default = new environment();
        /// <summary>
        /// 环境检测
        /// </summary>
        /// <returns></returns>
        public static bool Check()
        {
            DirectoryInfo jsDirectory = new DirectoryInfo(@"..\..\..\ui\js\");
            if (jsDirectory.Exists) copyJs(jsDirectory, new DirectoryInfo(@"..\..\js\"), true);
            FileInfo loadJs = new FileInfo(@"..\..\js\loadPage.js");
            if (!loadJs.Exists)
            {
#if VS2010
        Console.WriteLine(@"错误：没有找到必要的脚本文件 " + loadJs.FullName + @"
你可能需要先成功编译 fastCSharp.ui.vs2010 项目，然后再 重新编译 此项目。");
#else
                Console.WriteLine(@"错误：没有找到必要的脚本文件 " + loadJs.FullName + @"
你可能需要先成功编译 fastCSharp.ui 项目，然后再 重新编译 此项目。");
#endif
                return false;
            }
            FileInfo aceJs = new FileInfo(@"..\..\js\ace\mode-csharp.js");
            if (!aceJs.Exists)
            {
                FileInfo aceZip = new FileInfo(@"..\..\js\ace.zip");
                if (!aceZip.Exists)
                {
                    Console.WriteLine(@"警告：代码编辑组件 ACE 缺少某些文件，正在尝试下载 https://www.51nod.com/upload/ace.zip");
                    byte[] data;
                    using (fastCSharp.net.webClient client = new fastCSharp.net.webClient()) data = client.CrawlData("https://www.51nod.com/upload/ace.zip");
                    if (data.length() == 1867400)
                    {
                        File.WriteAllBytes(aceZip.FullName, data);
                        Console.WriteLine(@"代码编辑组件 ACE 下载成功");
                    }
                    else Console.WriteLine("代码编辑组件 ACE 下载失败。");
                }
                if ((aceZip = new FileInfo(aceZip.FullName)).Exists)
                {
                    string jsPath = new DirectoryInfo(@"..\..\js\").fullName();
                    using (FileStream stream = aceZip.OpenRead())
                    using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read, false))
                    {
                        foreach (ZipArchiveEntry zipEntry in zip.Entries)
                        {
                            if (zipEntry.Length != 0)
                            {
                                FileInfo file = new FileInfo(jsPath + zipEntry.FullName);
                                if (!file.Exists)
                                {
                                    DirectoryInfo directory = file.Directory;
                                    if (!directory.Exists) directory.Create();
                                    using (Stream aceFileStream = new FileStream(file.FullName, FileMode.Create))
                                    using (Stream zipStream = zipEntry.Open())
                                    {
                                        zipStream.CopyTo(aceFileStream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            FileInfo fastCSharpSln = new FileInfo(@"..\..\..\fastCSharp.sln");
            if (fastCSharpSln.Exists) Default.FastCSharpPath = fastCSharpSln.Directory.fullName();
            else Console.WriteLine(@"警告：没有找到 fastCSharp 解决方案文件 " + fastCSharpSln.FullName);
            return true;
        }
        /// <summary>
        /// 脚本文件复制
        /// </summary>
        /// <param name="formDirectory"></param>
        /// <param name="toDirectory"></param>
        /// <param name="isBoot"></param>
        private static void copyJs(DirectoryInfo formDirectory, DirectoryInfo toDirectory, bool isBoot)
        {
            if (!toDirectory.Exists) toDirectory.Create();
            string toPath = toDirectory.fullName();
            Dictionary<string, FileInfo> files = toDirectory.GetFiles().getDictionary(value => value.Name);
            foreach (FileInfo file in formDirectory.GetFiles())
            {
                FileInfo toFile;
                if (!files.TryGetValue(file.Name, out toFile) || file.LastWriteTimeUtc != toFile.LastWriteTimeUtc)
                {
                    string toFileName = toFile == null ? toPath + file.Name : toFile.FullName;
                    file.CopyTo(toFileName, true);
                    (toFile ?? new FileInfo(toFileName)).Attributes &= ~FileAttributes.ReadOnly;
                }
            }
            foreach (DirectoryInfo subDirectory in formDirectory.GetDirectories())
            {
                if (!isBoot|| subDirectory.Name == "ace") copyJs(subDirectory, new DirectoryInfo(toPath + subDirectory.Name), false);
            }
        }
    }
}
