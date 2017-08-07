using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using fastCSharp;

namespace fastCSharp.test.pack
{
    /// <summary>
    /// ZIP打包
    /// </summary>
    class zip
    {
        /// <summary>
        /// ZIP压缩包
        /// </summary>
        private ZipArchive zipArchive;
        /// <summary>
        /// ZIP打包
        /// </summary>
        /// <param name="path"></param>
        /// <param name="saveFileName"></param>
        public zip(string path, string saveFileName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    zipArchive = zip;
                    this.path(new DirectoryInfo(path), string.Empty);
                }
                using (FileStream packFile = new FileStream(saveFileName, FileMode.Create)) packFile.Write(stream.GetBuffer(), 0, (int)stream.Position);
            }
            Console.WriteLine(new FileInfo(saveFileName).FullName);
        }
        /// <summary>
        /// TFS结束
        /// </summary>
        private const string endGlobalSection = @"	EndGlobalSection
";
        /// <summary>
        /// 递归处理目录
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        private void path(DirectoryInfo directory, string path)
        {
            file(directory, path);
            foreach (DirectoryInfo nextDircectory in directory.GetDirectories())
            {
                int isPath = 1;
                switch (nextDircectory.Name)
                {
                    case "bin": case "obj": case ".vs": isPath = 0; break;
                    case "other": isPath = path.Length; break;
                    case "packages": isPath = path.Length == 0 || path == @"platform\" ? 0 : 1; break;
                    case "js":
                        if (path == @"ui\")
                        {
                            js(nextDircectory, path + nextDircectory.Name + @"\");
                            isPath = 0;
                        }
                        else if (!path.StartsWith(@"platform\", StringComparison.Ordinal)) isPath = 0;
                        break;
                }
                if (isPath != 0) this.path(nextDircectory, path + nextDircectory.Name + @"\");
            }
        }
        /// <summary>
        /// 文件处理
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        private void file(DirectoryInfo directory, string path)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name[0] == '%')
                {
                    file.Attributes = 0;
                    file.Delete();
                }
                else if (!file.Extension.StartsWith(".vs", StringComparison.Ordinal)
                    && !file.Extension.StartsWith(".suo", StringComparison.Ordinal))// && !file.Extension.StartsWith(".jar", StringComparison.Ordinal)
                {
                    string fileName = file.Name;
                    using (Stream entryStream = zipArchive.CreateEntry(path + fileName).Open())
                    {
                        byte[] data;
                        if (file.Extension == ".sln")
                        {
                            string content = File.ReadAllText(file.FullName, Encoding.UTF8);
                            int index = content.IndexOf(@"	GlobalSection(TeamFoundationVersionControl) = preSolution
");
                            if (index != -1)
                            {
                                int endIndex = content.IndexOf(endGlobalSection, index);
                                if (endIndex != -1) content = content.Remove(index, endIndex - index + endGlobalSection.Length);
                            }
                            data = Encoding.UTF8.GetBytes(content);
                        }
                        else data = File.ReadAllBytes(file.FullName);
                        entryStream.Write(data, 0, data.Length);
                    }
                }
            }
        }
        /// <summary>
        /// JS脚本文件处理
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        private void js(DirectoryInfo directory, string path)
        {
            file(directory, path);
            foreach (DirectoryInfo nextDircectory in directory.GetDirectories())
            {
                switch (nextDircectory.Name)
                {
                    case "ace": file(nextDircectory.fullName(), path + nextDircectory.Name + @"\", new string[] { "ace.js", "load.js", "load.ts" }); break;
                    case "mathJax": file(nextDircectory.fullName(), path + nextDircectory.Name + @"\", new string[] { "MathJax.js", "load.js", "load.ts" }); break;
                    case "highcharts": this.path(nextDircectory, path + nextDircectory.Name + @"\"); break;
                    default: fastCSharp.log.Error.ThrowReal("未知的js文件夹 " + nextDircectory.fullName(), new System.Diagnostics.StackFrame(), false); ; break;
                }
            }
        }
        /// <summary>
        /// 文件过滤处理
        /// </summary>
        /// <param name="dircectory"></param>
        /// <param name="path"></param>
        /// <param name="fileNames"></param>
        private void file(string dircectory, string path, string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                FileInfo file = new FileInfo(dircectory + fileName);
                if (file.Exists)
                {
                    using (Stream entryStream = zipArchive.CreateEntry(path + fileName).Open())
                    {
                        byte[] data = File.ReadAllBytes(file.FullName);
                        entryStream.Write(data, 0, data.Length);
                    }
                }
            }
        }
    }
}
