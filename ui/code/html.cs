using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using fastCSharp.reflection;
using fastCSharp.io;

namespace fastCSharp.code
{
    /// <summary>
    /// HTML模板
    /// </summary>
    [auto(Name = "HTML模板", IsAuto = true, IsTemplate = false)]
    internal sealed class html : IAuto
    {
        /// <summary>
        /// CSS模板文件名
        /// </summary>
        private const string cssPageExtension = ".page.css";
        /// <summary>
        /// css+background
        /// </summary>
        private static readonly Regex cssBackgroundRegex = new Regex(@"background(\-image)?\:url\((\.\.)?\/images\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        /// <summary>
        /// HTML模板处理
        /// </summary>
        private sealed class htmlJs
        {
            /// <summary>
            /// HTML文件扩展名
            /// </summary>
            private const string htmlExtension = ".html";
            /// <summary>
            /// JS文件扩展名
            /// </summary>
            private const string jsExtension = ".js";
            /// <summary>
            /// TypeScript文件扩展名
            /// </summary>
            private const string tsExtension = ".ts";
            /// <summary>
            /// 模板扩展名
            /// </summary>
            private const string pageExtension = ".page";
            /// <summary>
            /// HTML包含前缀
            /// </summary>
            private static readonly string[] htmlInclude = new string[] { "<!--include:" };
            /// <summary>
            /// js包含前缀
            /// </summary>
            private static readonly string[] jsInclude = new string[] { "/*include:" };
            /// <summary>
            /// 换行符
            /// </summary>
            private static readonly Regex enterRegex = new Regex(@"[\r\n]+", RegexOptions.Compiled);
            /// <summary>
            /// select控件
            /// </summary>
            private static readonly Regex selectRegex = new Regex(@"<\/?select", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            /// <summary>
            /// img控件
            /// </summary>
            private static readonly Regex imageRegex = new Regex(@" @?src\=""\/images\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            /// <summary>
            /// img控件
            /// </summary>
            private static readonly Regex imageSrcRegex = new Regex(@"<\!\-\-ImageSrc\-\-><img @src\=", RegexOptions.Compiled);
            /// <summary>
            /// link+css控件
            /// </summary>
            private static readonly Regex cssRegex = new Regex(@" href\=""\/css\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            /// <summary>
            /// HTML模板参数
            /// </summary>
            private html htmlAuto;
            /// <summary>
            /// 项目路径
            /// </summary>
            private string includePath;
            /// <summary>
            /// 项目路径
            /// </summary>
            internal string IncludePath
            {
                get { return includePath ?? htmlAuto.parameter.ProjectPath; }
            }
            /// <summary>
            /// 文件名称前缀
            /// </summary>
            public string fileName;
            /// <summary>
            /// 目标HTML代码
            /// </summary>
            private string html;
            /// <summary>
            /// 目标JS代码
            /// </summary>
            private string js;
            /// <summary>
            /// 包含标识集合
            /// </summary>
            private list<string> includes = new list<string>();
            /// <summary>
            /// JS文件集合
            /// </summary>
            private list<htmlJs> jsFiles;
            /// <summary>
            /// HTML模板处理
            /// </summary>
            /// <param name="htmlAuto">HTML模板参数</param>
            /// <param name="fileName">文件名称</param>
            /// <param name="includePath">导入 HTML/JS 目录</param>
            private unsafe htmlJs(html htmlAuto, string fileName, string includePath = null)
            {
                this.htmlAuto = htmlAuto;
                this.fileName = fileName;
                this.includePath = includePath;
            }
            /// <summary>
            /// 创建HTML文件
            /// </summary>
            /// <param name="history">历史记录</param>
            /// <param name="isHtml">是否HTML</param>
            private void create(list<htmlJs> history, bool isHtml)
            {
                #region 循环引用检测
                if (isHtml)
                {
                    if (history.contains(this))
                    {
                        fastCSharp.log.Default.ThrowReal(@"HTML循环引用:
" + history.joinString(@"
", value => value.fileName) + @"
" + fileName, null, false);
                    }
                    history.Add(this);
                }
                #endregion
                if (jsFiles == null)
                {
                    #region javascript
                    jsFiles = new list<htmlJs>();
                    string code = null, pageJsFileName = fileName + pageExtension + jsExtension;
                    if (File.Exists(pageJsFileName)) code = File.ReadAllText(pageJsFileName, File.Exists(fileName + pageExtension + tsExtension) ? Encoding.UTF8 : htmlAuto.webConfig.Encoding);
                    else
                    {
                        string targetJsFileName = fileName + jsExtension;
                        if (File.Exists(targetJsFileName)) code = File.ReadAllText(targetJsFileName, File.Exists(fileName + tsExtension) ? Encoding.UTF8 : htmlAuto.webConfig.Encoding);
                    }
                    if (code != null)
                    {
                        string[] codes = code.ToLower().Split(jsInclude, StringSplitOptions.None);
                        subArray<string> newCode = new subArray<string>(codes.Length);
                        int startIndex = codes[0].Length, jsIncludeLength = jsInclude[0].Length, index = 0;
                        newCode.Add(code.Substring(0, startIndex));
                        foreach (string splitCode in codes)
                        {
                            if (index != 0)
                            {
                                int splitIndex = splitCode.IndexOf("*/", StringComparison.Ordinal);
                                if (splitIndex == -1) fastCSharp.log.Default.ThrowReal("非法标签:" + jsInclude[0] + splitCode, null, false);
                                htmlJs file;
                                if (htmlAuto.htmls.TryGetValue(new FileInfo(IncludePath + splitCode.Substring(0, splitIndex)).FullName.toLower(), out file))
                                {
                                    file.create(history, false);
                                    foreach (htmlJs value in file.jsFiles)
                                    {
                                        if (!jsFiles.Contains(value)) jsFiles.Add(value);
                                    }
                                    newCode.Add(code.Substring(startIndex + jsIncludeLength + (splitIndex += 2), splitCode.Length - splitIndex));
                                    startIndex += jsIncludeLength + splitCode.Length;
                                }
                                else fastCSharp.log.Default.ThrowReal("未找到文件:" + IncludePath + splitCode.Substring(0, splitIndex) + jsExtension + @"
in " + this.fileName + @"
" + splitCode, new System.Diagnostics.StackFrame(), false);
                            }
                            ++index;
                        }
                        js = string.Concat(newCode.ToArray());
                    }
                    jsFiles.Add(this);
                    #endregion
                    #region html
                    string pageFileName = fileName + pageExtension + htmlExtension, targetFileName = fileName + htmlExtension;
                    if (pageFileName != null && File.Exists(pageFileName)) code = File.ReadAllText(pageFileName, htmlAuto.webConfig.Encoding);
                    else if (File.Exists(targetFileName)) code = File.ReadAllText(targetFileName, htmlAuto.webConfig.Encoding);
                    else code = null;
                    if (code != null)
                    {
                        string[] codes = code.ToLower().Split(htmlInclude, StringSplitOptions.None);
                        list<string> newCode = new list<string>(codes.Length << 1);
                        Dictionary<hashString, int> includeIndexs = dictionary.CreateHashString<int>();
                        int startIndex = codes[0].Length, htmlIncludeLength = htmlInclude[0].Length, index = 0;
                        newCode.Add(code.Substring(0, startIndex));
                        foreach (string splitCode in codes)
                        {
                            if (index != 0)
                            {
                                int splitIndex = splitCode.IndexOf("-->", StringComparison.Ordinal);
                                if (splitIndex == -1) fastCSharp.log.Default.ThrowReal("非法标签:" + htmlInclude[0] + splitCode, null, false);
                                subString name = subString.Unsafe(code, startIndex + htmlIncludeLength, splitIndex), memberName = default(subString);
                                subArray<subString> array = default(subArray<subString>);
                                bool isHash = false;
                                if (name.Length != 0 && name[0] == '#')
                                {
                                    isHash = true;
                                    name = subString.Unsafe(code, name.StartIndex + 1, name.Length - 1);
                                }
                                else
                                {
                                    if (name[name.Length - 1] == ']')
                                    {
                                        int arrayIndex = name.IndexOf('[');
                                        array = name.Substring(arrayIndex + 1, name.Length - arrayIndex - 2).Split(',');
                                        name = name.Substring(0, arrayIndex);
                                    }
                                    int memberIndex = name.IndexOf('=');
                                    if (memberIndex != -1)
                                    {
                                        memberName = name.Substring(0, memberIndex);
                                        name = name.Substring(++memberIndex);
                                        if (memberIndex == 1) memberName = name.Split('\\').LastOrDefault();
                                        memberName = @"<!--Value:" + memberName + @"-->";
                                    }
                                }
                                htmlJs file;
                                if (htmlAuto.htmls.TryGetValue(new FileInfo(IncludePath + name).FullName.toLower(), out file))
                                {
                                    file.create(history, true);
                                    foreach (htmlJs value in file.jsFiles)
                                    {
                                        if (!jsFiles.Contains(value)) jsFiles.Add(value);
                                    }
                                    if (memberName.Value != null)
                                    {
                                        newCode.Add(memberName + @"
");
                                    }
                                    string fileHtml = file.html;
                                    if (array.Count != 0)
                                    {
                                        for (int arrayIndex = array.Count - 1; arrayIndex >= 0; --arrayIndex)
                                        {
                                            fileHtml = fileHtml.Replace("=@[" + arrayIndex.ToString() + "]", array[arrayIndex]);
                                        }
                                    }
                                    if (isHash)
                                    {
                                        if (!includes.Contains(fileHtml))
                                        {
                                            hashString fileKey = fileHtml;
                                            if (!includeIndexs.ContainsKey(fileKey))
                                            {
                                                includeIndexs[fileKey] = newCode.Count;
                                                newCode.Add((string)null);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string include in file.includes) includes.Add(fileHtml);
                                        newCode.Add(fileHtml);
                                    }
                                    if (memberName != null)
                                    {
                                        newCode.Add(@"
" + memberName);
                                    }
                                    newCode.Add(code.Substring(startIndex + htmlIncludeLength + (splitIndex += 3), splitCode.Length - splitIndex));
                                    startIndex += htmlIncludeLength + splitCode.Length;
                                }
                                else fastCSharp.log.Default.ThrowReal("未找到文件:" + IncludePath + splitCode.Substring(0, splitIndex) + htmlExtension + @"
in " + this.fileName + @"", new System.Diagnostics.StackFrame(), false);
                            }
                            ++index;
                        }
                        foreach (KeyValuePair<hashString, int> include in includeIndexs)
                        {
                            string fileHtml = include.Key.ToString();
                            if (!includes.Contains(fileHtml))
                            {
                                includes.Add(fileHtml);
                                newCode[include.Value] = fileHtml;
                            }
                        }
                        html = string.Concat(newCode.ToArray())
                            .Replace(@"<img src=""", @"<img @src=""")
                            .Replace(@" src=""=@", @" @src=""=@")
                            .Replace(@" style=""", @" @style=""")
                            .Replace(@" check=""=@", @" @check=""=@");
                        if (includePath == null && File.Exists(pageFileName))
                        {
                            string writeHtml = selectRegex.Replace(html, match => match.Value.Length == 7 ? "<select@" : "</select@");
                            //int bodyIndex = writeHtml.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);
                            //if (bodyIndex != -1) writeHtml = writeHtml.Substring(0, bodyIndex) + cSharp.webView.treeBuilder.ViewBody + writeHtml.Substring(bodyIndex);
                            codes = writeHtml.Split(new string[] { "<!--NOBR-->" }, StringSplitOptions.None);
                            for (int bodyIndex = 1; bodyIndex < codes.Length; bodyIndex += 2) codes[bodyIndex] = enterRegex.Replace(codes[bodyIndex], string.Empty);
                            if (codes.Length > 1) writeHtml = string.Concat(codes);
                            writeFile(targetFileName, writeHtml);
                        }
                    }
                    #endregion
                    jsFiles.Remove(this);
                    jsFiles.Add(this);
                    if (includePath == null)
                    {
                        code = jsFiles.joinString(@"
", file => file.js);
                        if (code.Length != 0)
                        {
                            if (fileName == htmlAuto.parameter.ProjectPath + (@"js\base").pathSeparator())
                            {
                                DirectoryInfo viewDirectory = new DirectoryInfo(htmlAuto.parameter.ProjectPath + htmlAuto.webConfig.ViewJsDirectory + Path.DirectorySeparatorChar);
                                if (viewDirectory.Exists)
                                {
                                    FileInfo[] viewJsFiles = viewDirectory.GetFiles("*" + jsExtension, SearchOption.TopDirectoryOnly);
                                    FileInfo viewJsFileInfo = new FileInfo(viewDirectory.fullName() + "view.js");
                                    string viewJs = null;
                                    jsFiles.Empty();
                                    if (viewJsFileInfo.Exists)
                                    {
                                        viewJs = viewJsFileInfo.FullName;
                                        htmlJs file = htmlAuto.htmls[viewJs = viewJs.Substring(0, viewJs.Length - jsExtension.Length).toLower()];
                                        file.create(history, false);
                                        foreach (htmlJs value in file.jsFiles)
                                        {
                                            if (!jsFiles.Contains(value)) jsFiles.Add(value);
                                        }
                                    }
                                    foreach (FileInfo viewJsFile in viewJsFiles)
                                    {
                                        string jsFile = viewJsFile.FullName;
                                        if ((jsFile = jsFile.Substring(0, jsFile.Length - jsExtension.Length).toLower()) != viewJs)
                                        {
                                            htmlJs file = htmlAuto.htmls[jsFile];
                                            file.create(history, false);
                                            foreach (htmlJs value in file.jsFiles)
                                            {
                                                if (!jsFiles.Contains(value)) jsFiles.Add(value);
                                            }
                                        }
                                    }
                                    viewJs = jsFiles.joinString(@"
", file => file.js);
                                    if (viewJs.Length != 0)
                                    {
                                        int index = code.IndexOf(@"setTimeout(fastCSharp.Pub.LoadIE,");
                                        code = code.Insert(index < 0 ? 0 : index, viewJs + @"
");
                                    }
                                }
                                writeFile(fileName + jsExtension, compress(code));
                            }
                            else if (fileName == htmlAuto.parameter.ProjectPath + (@"js\load").pathSeparator())
                            {
                                string charset = "Loader.Charset='" + htmlAuto.webConfig.Encoding.WebName + "';";
                                string pageCode = code.Replace("Loader.PageView = false;", "Loader.PageView = true;" + charset);
                                code = code.Replace("Loader.PageView = false;", charset);
                                writeFile(fileName + jsExtension, compress(code));
                                writeFile(fileName + "Page" + jsExtension, compress(pageCode));
                            }
                            else if (File.Exists(pageJsFileName)) writeFile(fileName + jsExtension, compress(code));
                        }
                    }
                }
                if (isHtml) history.UnsafeAddLength(-1);
            }
            /// <summary>
            /// 写入目标文件
            /// </summary>
            /// <param name="fileName">目标文件名称</param>
            /// <param name="code">文件内容</param>
            private void writeFile(string fileName, string code)
            {
                code = imageRegex.Replace(code, @" @src=""//" + htmlAuto.webConfig.StaticFileDomain + "/images/");
                code = imageSrcRegex.Replace(code, "<img src=");
                if (htmlAuto.webConfig.IsCssStaticFileDomain) code = cssRegex.Replace(code, @" href=""//" + htmlAuto.webConfig.StaticFileDomain + "/css/");
                code = code.Replace("__MAINDOMAIN__", htmlAuto.webConfig.MainDomain)
                    .Replace("__STATICDOMAIN__", htmlAuto.webConfig.StaticFileDomain)
                    .Replace("__IMAGEDOMAIN__", htmlAuto.webConfig.ImageDomain)
                    .Replace("__JSON__", fastCSharp.config.web.QueryJsonName.ToString())
                    .Replace("__XML__", fastCSharp.config.web.QueryXmlName.ToString())
                    .Replace("__VERSION__", htmlAuto.version);
                if (!File.Exists(fileName) || File.ReadAllText(fileName, htmlAuto.webConfig.Encoding) != code)
                {
                    File.WriteAllText(fileName, code, htmlAuto.webConfig.Encoding);
                }
            }
            /// <summary>
            /// 创建HTML模板目标文件
            /// </summary>
            /// <param name="htmlAuto">HTML模板参数</param>
            /// <returns></returns>
            public static bool Create(html htmlAuto)
            {
                foreach (string include in htmlAuto.webConfig.IncludeDirectories.notNull())
                {
                    DirectoryInfo directory = new DirectoryInfo(include.IndexOf(':') == -1 ? htmlAuto.parameter.ProjectPath + include : include);
                    if (directory.Exists)
                    {
                        string path = directory.fullName();
                        foreach (string fileName in Directory.GetFiles(path, "*" + htmlExtension, SearchOption.AllDirectories))
                        {
                            if (!fileName.EndsWith(pageExtension + htmlExtension)) create(htmlAuto, fileName, true, path);
                        }
                        foreach (string fileName in Directory.GetFiles(path, "*" + jsExtension, SearchOption.AllDirectories))
                        {
                            if (!fileName.EndsWith(pageExtension + jsExtension)) create(htmlAuto, fileName, false, path);
                        }
                    }
                    else
                    {
                        error.Message("没有找到附加导入 HTML/JS 目录 " + include + " => " + directory.FullName);
                        return false;
                    }
                }
                foreach (string fileName in Directory.GetFiles(htmlAuto.parameter.ProjectPath, "*" + htmlExtension, SearchOption.AllDirectories))
                {
                    create(htmlAuto, fileName, true);
                }
                foreach (string fileName in Directory.GetFiles(htmlAuto.parameter.ProjectPath, "*" + jsExtension, SearchOption.AllDirectories))
                {
                    create(htmlAuto, fileName, false);
                }
                foreach (htmlJs value in htmlAuto.htmls.Values) value.create(new list<htmlJs>(), true);
                return true;
            }
            /// <summary>
            /// 添加HTML模板处理
            /// </summary>
            /// <param name="htmlAuto">HTML模板参数</param>
            /// <param name="fileName">文件名称</param>
            /// <param name="isHtml">是否HTML文件</param>
            /// <param name="includePath">导入 HTML/JS 目录</param>
            private static unsafe void create(html htmlAuto, string fileName, bool isHtml, string includePath = null)
            {
                subString name = subString.Unsafe(fileName, 0, fileName.Length - (isHtml ? htmlExtension : jsExtension).Length);
                fixed (char* nameFixed = fileName, pageFixed = pageExtension)
                {
                    if (unsafer.memory.SimpleEqual((byte*)(nameFixed + name.Length - pageExtension.Length), (byte*)pageFixed, pageExtension.Length << 1))
                    {
                        string jsFile = name + jsExtension;
                        if (!File.Exists(jsFile)) File.WriteAllText(jsFile, string.Empty, htmlAuto.webConfig.Encoding);
                        name = subString.Unsafe(fileName, 0, name.Length - pageExtension.Length);
                    }
                }
                hashString nameKey = (fileName = name).ToLower();
                if (!htmlAuto.htmls.ContainsKey(nameKey)) htmlAuto.htmls.Add(nameKey, new htmlJs(htmlAuto, fileName, includePath));
            }
            /// <summary>
            /// 多行注释正则
            /// </summary>
            private static readonly Regex multiLineNodes = new Regex(@"\/\*[\u0000-\uffff]*?\*\/[\r|\n]+", RegexOptions.Compiled);
            /// <summary>
            /// 单行注释正则
            /// </summary>
            private static readonly Regex singleLineNodes = new Regex(@"[\r|\n]+[\t ]*\/\/[^\r\n]*", RegexOptions.Compiled);
            /// <summary>
            /// TypeScript引用注释
            /// </summary>
            private static readonly Regex referencePathRegex = new Regex(@"\/\/\/ ?\<reference +path ?\= ?""[^\n]+[\r\n]+");
            /// <summary>
            /// js严格模式
            /// </summary>
            private const string useStrict = @"'use strict';
";
            /// <summary>
            /// 继承代码定义
            /// </summary>
            private const string extends = @"var __extends = (this && this.__extends) || function (d, b) {
	for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
	function __() { this.constructor = d; }
	d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
";
            /// <summary>
            /// 压缩源代码//showjim应该修改为trie图匹配
            /// </summary>
            /// <param name="code">源代码</param>
            /// <returns>压缩后源代码</returns>
            private unsafe string compress(string code)
            {
                code = code.Replace("    ", "\t").Replace(extends, string.Empty).Replace("__extends", "fastCSharp.Pub.Extends")
                    .Replace("__MAINDOMAIN__", htmlAuto.webConfig.MainDomain)
                    .Replace("__POLLDOMAIN__", htmlAuto.webConfig.PollDomain)
                    .Replace("__VIEWLOCATION__", htmlAuto.webConfig.NoViewLocation)
                    .Replace("__STATICDOMAIN__", htmlAuto.webConfig.StaticFileDomain)
                    .Replace("__IMAGEDOMAIN__", htmlAuto.webConfig.ImageDomain)
                    .Replace("__AJAXRETURN__", fastCSharp.net.returnValue.ReturnParameterName)
                    .Replace("__AJAX__", fastCSharp.config.web.Default.AjaxWebCallName)
                    .Replace("__AJAXCALL__", fastCSharp.config.web.AjaxCallName.ToString())
                    .Replace("__CALLBACK__", fastCSharp.config.web.AjaxCallBackName.ToString())
                    .Replace("__JSON__", fastCSharp.config.web.QueryJsonName.ToString())
                    .Replace("__XML__", fastCSharp.config.web.QueryXmlName.ToString())
                    .Replace("__REVIEW__", fastCSharp.config.web.ReViewName.ToString())
                    .Replace("__MOBILEREVIEW__", fastCSharp.config.web.MobileReViewName.ToString())
                    .Replace("__VIEWONLY__", fastCSharp.config.web.ViewOnlyName.ToString())
                    .Replace("__LOADPAGECACHE__", fastCSharp.config.web.LoadPageCache.ToString())
                    .Replace("__PUBERROR__", fastCSharp.code.cSharp.ajax.PubErrorCallName)
                    ;
                code = multiLineNodes.Replace(code, string.Empty);
                code = singleLineNodes.Replace(code, string.Empty);
                code = referencePathRegex.Replace(code, string.Empty);
                int index = code.IndexOf(useStrict);
                if (index != -1) code = code.Substring(0, index += useStrict.Length) + code.Substring(index).Replace(useStrict, string.Empty);
                return code;
                //code = (new Regex(@"\r\n\t*")).Replace(code, string.Empty);
                //code = (new Regex("[\r\n]*")).Replace(code, string.Empty);
                //code = (new Regex(@"([\)\{\;\=])\t+")).Replace(code, "$1").Replace("	{", "{");
                //string code1 = SingleCharCompress(code), code2 = DoubleCharCompress(code);
                //return code1.Length < code.Length ? (code1.Length < code2.Length ? code1 : code2) : (code2.Length < code.Length ? code2 : code);
            }
            //showjim
            //#region 获取单字符压缩代码
            ///// <summary>
            ///// 获取单字符压缩代码
            ///// </summary>
            ///// <param name="code">源代码</param>
            ///// <returns>压缩代码</returns>
            //private static string SingleCharCompress(string code)
            //{
            //    #region 获取重复单词集合
            //    string split = @"; ().='{}+[],""?:*-><	_!&/|%\^@$#", substring;
            //    char escapeChar = '\\', quoteChar = '"', splitChar = split[split.Length - 1];
            //    int startIndex = 0, length, charIndex = 0, charCode, maxCharCode = 127, quoteCode = (int)quoteChar, escapeCode = (int)escapeChar, splitCode = (int)splitChar, count, freeCount = 0;
            //    list<string> removeKey;
            //    Dictionary<string, list<int>> allCount = dictionary.CreateOnly<string, list<int>>();
            //    int[] charCount = new int[maxCharCode];
            //    for (int i = maxCharCode - 1; i >= 0; i--) charCount[i] = ((i >= 32 && i != quoteCode && i != escapeCode) || i == 7 ? 0 : 1);
            //    foreach (char codeChar in code)
            //    {
            //        if ((charCode = (int)codeChar) >= maxCharCode || split.IndexOf(codeChar) != -1)
            //        {
            //            if ((length = charIndex - startIndex) >= 2)
            //            {
            //                if (!allCount.ContainsKey(substring = code.Substring(startIndex, length))) allCount.Add(substring, new list<int>());
            //                allCount[substring].Add(startIndex + length);
            //            }
            //            startIndex = charIndex + 1;
            //        }
            //        if (charCode < maxCharCode) charCount[charCode]++;
            //        charIndex++;
            //    }
            //    removeKey = new list<string>();
            //    foreach (string key in allCount.Keys)
            //    {
            //        if (allCount[key].Count == 1) removeKey.Add(key);
            //    }
            //    foreach (string key in removeKey) allCount.Remove(key);
            //    #endregion

            //    #region 字符串后缀检测
            //    bool isNext;
            //    char cmpChar, endChar = pub.NullChar;
            //    list<int> endIndex;
            //    Dictionary<string, list<int>> allEndCount = dictionary.CreateOnly<string, list<int>>();
            //    length = code.Length;
            //    foreach (string key in allCount.Keys)
            //    {
            //        isNext = true;
            //        endIndex = allCount[substring = key];
            //        cmpChar = pub.NullChar;
            //        while (isNext)
            //        {
            //            foreach (int index in endIndex)
            //            {
            //                if (isNext = (index < length && split.IndexOf(endChar = code[index]) != -1) && endChar != splitCode)
            //                {
            //                    if (cmpChar == pub.NullChar) cmpChar = endChar;
            //                    else isNext = (cmpChar == endChar);
            //                }
            //                if (!isNext) break;
            //            }
            //            if (isNext)
            //            {
            //                substring += cmpChar;
            //                for (int i = endIndex.Count - 1; i >= 0; i--) endIndex[i]++;
            //            }
            //        }
            //        allEndCount.Add(substring, endIndex);
            //    }
            //    #endregion

            //    #region 计算压缩量并删除压缩不了的单词
            //    Dictionary<string, int> freeCounts = dictionary.CreateOnly<string, int>();
            //    removeKey = new list<string>();
            //    foreach (string key in allEndCount.Keys)
            //    {
            //        if ((count = key.Length * allEndCount[key].Count - (key.Length << 1) - allEndCount[key].Count - 2 + key.Replace(escapeChar.ToString(), string.Empty).Replace(quoteChar.ToString(), string.Empty).Length) > 0)
            //        {
            //            freeCounts.Add(key, count);
            //            freeCount += count;
            //        }
            //        else removeKey.Add(key);
            //    }
            //    foreach (string key in removeKey) allEndCount.Remove(key);
            //    #endregion

            //    if (allEndCount.Count != 0 && freeCount > 101)
            //    {
            //        #region 计算总压缩量并生成单词序列
            //        bool isFree, isFreeKey;
            //        freeCount = -185;
            //        list<string> freeKeys = new list<string>();
            //        string freeChars = string.Empty, freeKey;
            //        do
            //        {
            //            isFreeKey = false;
            //            #region 处理独占字符的单词
            //            for (int i = charCount.Length - 1; i >= 7 && freeCounts.Count != 0; i--)
            //            {
            //                if (charCount[i] > 0)
            //                {
            //                    removeKey = new list<string>();
            //                    foreach (string key in freeCounts.Keys)
            //                    {
            //                        if ((count = allEndCount[key].Count) == charCount[i] && key.IndexOf((char)i) != -1)
            //                        {
            //                            isFreeKey = true;
            //                            freeKeys.Add(key);
            //                            freeChars += (char)i;
            //                            foreach (char freeChar in key) charCount[(int)freeChar] -= count;
            //                            freeCount += freeCounts[key];
            //                            removeKey.Add(key);
            //                            charCount[i] = -1;
            //                        }
            //                    }
            //                    foreach (string key in removeKey) freeCounts.Remove(key);
            //                }
            //            }
            //            #endregion

            //            #region 为空余字符分配单词
            //            do
            //            {
            //                isFree = false;
            //                for (int i = charCount.Length - 1; i >= 7 && freeCounts.Count != 0; i--)
            //                {
            //                    if (charCount[i] == 0)
            //                    {
            //                        isFreeKey = true;
            //                        freeKey = null;
            //                        foreach (string key in freeCounts.Keys)
            //                        {
            //                            if (freeKey == null || freeCounts[key] > freeCounts[freeKey]) freeKey = key;
            //                        }
            //                        count = allEndCount[freeKey].Count;
            //                        foreach (char freeChar in freeKey)
            //                        {
            //                            if ((charCount[(int)freeChar] -= count) == 0) isFree = true;
            //                        }
            //                        freeCount += freeCounts[freeKey];
            //                        freeKeys.Add(freeKey);
            //                        freeCounts.Remove(freeKey);
            //                        freeChars += (char)i;
            //                        charCount[i] = -1;
            //                    }
            //                }
            //            }
            //            while (isFree && freeCounts.Count != 0);
            //            #endregion
            //        }
            //        while (isFreeKey && freeCounts.Count != 0);
            //        #endregion

            //        #region 压缩代码
            //        if ((freeCount -= charCount[quoteCode] + charCount[escapeCode]) > 0)
            //        {
            //            int oldLength = code.Length;
            //            stringBuilder createCode = new stringBuilder();
            //            createCode.Add(@"{var a=""");
            //            for (int i = freeChars.Length - 1; i >= 0; i--) createCode.Append(freeChars[i]);
            //            createCode.Add(@""",A=(""");
            //            for (int i = 0; i < freeChars.Length; i++)
            //            {
            //                freeKey = freeKeys[i];
            //                foreach (int index in allEndCount[freeKeys[i]])
            //                {
            //                    code = code.Remove(index - freeKey.Length, 2).Insert(index - freeKey.Length, pub.NullChar.ToString() + ((char)(i + 1)).ToString());
            //                }
            //            }
            //            count = 0;
            //            for (int i = freeChars.Length - 1; i >= 0; i--)
            //            {
            //                if (count == 0) count++;
            //                else createCode.Append(splitChar);
            //                createCode.Add(freeKeys[i].Replace(@"\", @"\\").Replace(@"""", @"\"""));
            //                code = code.Replace(pub.NullChar.ToString() + ((char)(i + 1)).ToString() + freeKeys[i].Substring(2), freeChars[i].ToString());
            //            }
            //            createCode.Append(@""").split('", splitChar.ToString(), "')");
            //            createCode.Append(@",c=""", code.Replace(@"\", @"\\").Replace(@"""", @"\"""), @""",i;");
            //            createCode.Add("for(i=0;i<a.length;i++)c=c.split(a.charAt(i)).join(A[i]);eval(c);}");
            //            string newCode = createCode.ToString();
            //            if (newCode.Length < oldLength) code = newCode;
            //        }
            //        #endregion
            //    }
            //    return code;
            //}
            //#endregion

            //#region 获取双字符压缩代码
            ///// <summary>
            ///// 获取双字符压缩代码
            ///// </summary>
            ///// <param name="code">源代码</param>
            ///// <returns>压缩代码</returns>
            //private static string DoubleCharCompress(string code)
            //{
            //    #region 获取重复单词集合
            //    string split = @"; ().='{}+[],""?:*-><	_!&/|%\^@$#", replaceCode = code, substring;
            //    char escapeChar = '\\', quoteChar = '"', splitChar = split[split.Length - 1], charSplitChar = '}';
            //    int startIndex = 0, length, charIndex = 0, charCode, maxCharCode = 127, quoteCode = (int)quoteChar, escapeCode = (int)escapeChar, splitCode = (int)splitChar, charSplitCode = (int)charSplitChar, count, freeCount = 0;
            //    list<string> removeKey;
            //    Dictionary<string, collection<int>> allCount = dictionary.CreateOnly<string, collection<int>>();
            //    int[] charCount = new int[maxCharCode];
            //    for (int i = maxCharCode - 1; i >= 0; i--) charCount[i] = ((i >= 32 && i != quoteCode && i != escapeCode && i != charSplitCode) || i == 7 ? 0 : 1);
            //    foreach (char codeChar in code)
            //    {
            //        if ((charCode = (int)codeChar) >= maxCharCode || split.IndexOf(codeChar) != -1)
            //        {
            //            if ((length = charIndex - startIndex) >= 2)
            //            {
            //                if (!allCount.ContainsKey(substring = code.Substring(startIndex, length))) allCount.Add(substring, new collection<int>());
            //                allCount[substring].Add(startIndex + length);
            //            }
            //            startIndex = charIndex + 1;
            //        }
            //        if (charCode < maxCharCode) charCount[charCode]++;
            //        charIndex++;
            //    }
            //    removeKey = new list<string>();
            //    foreach (string key in allCount.Keys)
            //    {
            //        if (allCount[key].Count == 1) removeKey.Add(key);
            //    }
            //    foreach (string key in removeKey) allCount.Remove(key);
            //    #endregion

            //    #region 字符串后缀检测
            //    bool isNext;
            //    char cmpChar, endChar = pub.NullChar;
            //    collection<int> endIndex;
            //    Dictionary<string, collection<int>> allEndCount = dictionary.CreateOnly<string, collection<int>>();
            //    length = code.Length;
            //    foreach (string key in allCount.Keys)
            //    {
            //        isNext = true;
            //        endIndex = allCount[substring = key];
            //        cmpChar = pub.NullChar;
            //        while (isNext)
            //        {
            //            foreach (int index in endIndex)
            //            {
            //                if (isNext = (index < length && split.IndexOf(endChar = code[index]) != -1) && endChar != splitCode)
            //                {
            //                    if (cmpChar == pub.NullChar) cmpChar = endChar;
            //                    else isNext = (cmpChar == endChar);
            //                }
            //                if (!isNext) break;
            //            }
            //            if (isNext)
            //            {
            //                substring += cmpChar;
            //                for (int i = endIndex.Count - 1; i >= 0; i--) endIndex[i]++;
            //            }
            //        }
            //        allEndCount.Add(substring, endIndex);
            //    }
            //    #endregion

            //    #region 计算压缩量并删除单字符压缩不了的单词
            //    Dictionary<string, int> singleFreeCounts = dictionary.CreateOnly<string, int>();
            //    removeKey = new list<string>();
            //    foreach (string key in allEndCount.Keys)
            //    {
            //        if ((count = key.Length * allEndCount[key].Count - (key.Length << 1) - allEndCount[key].Count - 3 + key.Replace(escapeChar.ToString(), string.Empty).Replace(quoteChar.ToString(), string.Empty).Length) > 0)
            //        {
            //            singleFreeCounts.Add(key, count);
            //            freeCount += count;
            //        }
            //        else removeKey.Add(key);
            //    }
            //    foreach (string key in removeKey) allEndCount.Remove(key);
            //    #endregion

            //    if (allEndCount.Count != 0 && freeCount > 94)
            //    {
            //        #region 计算单字符压缩量并生成单词序列
            //        bool isFree, isFreeKey;
            //        list<string> freeChars = new list<string>(), freeKeys = new list<string>();
            //        string freeKey;
            //        do
            //        {
            //            isFreeKey = false;
            //            #region 处理独占字符的单词
            //            for (int i = charCount.Length - 1; i >= 7 && singleFreeCounts.Count != 0; i--)
            //            {
            //                if (charCount[i] > 0)
            //                {
            //                    removeKey = new list<string>();
            //                    foreach (string key in singleFreeCounts.Keys)
            //                    {
            //                        if ((count = allEndCount[key].Count) == charCount[i] && key.IndexOf((char)i) != -1)
            //                        {
            //                            isFreeKey = true;
            //                            freeKeys.Add(key);
            //                            freeChars.Add(((char)i).ToString());
            //                            replaceCode = DoubleCharCompressReplace(replaceCode, allEndCount, key, ((char)i).ToString());
            //                            foreach (char freeChar in key) charCount[(int)freeChar] -= count;
            //                            removeKey.Add(key);
            //                            charCount[i] = -1;
            //                        }
            //                    }
            //                    foreach (string key in removeKey) singleFreeCounts.Remove(key);
            //                }
            //            }
            //            #endregion

            //            #region 为空余字符分配单词
            //            do
            //            {
            //                isFree = false;
            //                for (int i = charCount.Length - 1; i >= 7 && singleFreeCounts.Count != 0; i--)
            //                {
            //                    if (charCount[i] == 0)
            //                    {
            //                        isFreeKey = true;
            //                        freeKey = null;
            //                        foreach (string key in singleFreeCounts.Keys)
            //                        {
            //                            if (freeKey == null || singleFreeCounts[key] > singleFreeCounts[freeKey]) freeKey = key;
            //                        }
            //                        count = allEndCount[freeKey].Count;
            //                        foreach (char freeChar in freeKey)
            //                        {
            //                            if ((charCount[(int)freeChar] -= count) == 0) isFree = true;
            //                        }
            //                        freeKeys.Add(freeKey);
            //                        singleFreeCounts.Remove(freeKey);
            //                        freeChars.Add(((char)i).ToString());
            //                        replaceCode = DoubleCharCompressReplace(replaceCode, allEndCount, freeKey, ((char)i).ToString());
            //                        charCount[i] = -1;
            //                    }
            //                }
            //            }
            //            while (isFree && singleFreeCounts.Count != 0);
            //            #endregion
            //        }
            //        while (isFreeKey && singleFreeCounts.Count != 0);
            //        #endregion

            //        if (singleFreeCounts.Count != 0)
            //        {
            //            #region 计算双字符压缩量
            //            Dictionary<string, int> doubleFreeCounts = dictionary.CreateOnly<string, int>();
            //            foreach (string key in singleFreeCounts.Keys)
            //            {
            //                if ((count = key.Length * allEndCount[key].Count - ((key.Length + allEndCount[key].Count) << 1) - 4 + key.Replace(escapeChar.ToString(), string.Empty).Replace(quoteChar.ToString(), string.Empty).Length) > 0) doubleFreeCounts.Add(key, count);
            //            }
            //            #endregion
            //            if (doubleFreeCounts.Count != 0)
            //            {
            //                bool isFreeDoubleKey;
            //                int char1 = maxCharCode - 1, char2 = maxCharCode - 1;
            //                string charKey;
            //                do
            //                {
            //                    isFree = isFreeDoubleKey = false;
            //                    #region 生成双字符替换串
            //                    charKey = null;
            //                    while (char1 >= 7 && charKey == null)
            //                    {
            //                        while (char2 >= 7 && charKey == null)
            //                        {
            //                            if (char1 != char2 && replaceCode.IndexOf(charKey = ((char)char1).ToString() + ((char)char2).ToString()) != -1) charKey = null;
            //                            if (char2 == 32) char2 = 7;
            //                            else
            //                            {
            //                                if ((--char2) == quoteCode) char2--;
            //                                if (char2 == escapeCode) char2--;
            //                                if (char2 == charSplitCode) char2--;
            //                            }
            //                        }
            //                        if (charKey == null)
            //                        {
            //                            char2 = maxCharCode - 1;
            //                            if (char1 == 32) char1 = 7;
            //                            else
            //                            {
            //                                if ((--char1) == quoteCode) char1--;
            //                                if (char1 == escapeCode) char1--;
            //                                if (char1 == charSplitCode) char1--;
            //                            }
            //                        }
            //                    }
            //                    if (charKey == null)
            //                    {
            //                        char1 = char2 = maxCharCode - 1;
            //                        while (char1 >= 7 && charKey == null)
            //                        {
            //                            while (char2 >= 7 && charKey == null)
            //                            {
            //                                if (char1 != char2 && replaceCode.IndexOf(charKey = ((char)char1).ToString() + ((char)char2).ToString()) != -1) charKey = null;
            //                                if (char2 == 32) char2 = 7;
            //                                else
            //                                {
            //                                    if ((--char2) == quoteCode) char2--;
            //                                    if (char2 == escapeCode) char2--;
            //                                    if (char2 == charSplitCode) char2--;
            //                                }
            //                            }
            //                            if (charKey == null)
            //                            {
            //                                char2 = maxCharCode - 1;
            //                                if (char1 == 32) char1 = 7;
            //                                else
            //                                {
            //                                    if ((--char1) == quoteCode) char1--;
            //                                    if (char1 == escapeCode) char1--;
            //                                    if (char1 == charSplitCode) char1--;
            //                                }
            //                            }
            //                        }
            //                    }
            //                    #endregion
            //                    if (charKey != null)
            //                    {
            //                        isFreeDoubleKey = true;
            //                        freeKey = null;
            //                        foreach (string key in doubleFreeCounts.Keys)
            //                        {
            //                            if (freeKey == null || doubleFreeCounts[key] > doubleFreeCounts[freeKey]) freeKey = key;
            //                        }
            //                        count = allEndCount[freeKey].Count;
            //                        freeKeys.Add(freeKey);
            //                        freeChars.Add(charKey);
            //                        replaceCode = DoubleCharCompressReplace(replaceCode, allEndCount, freeKey, charKey);
            //                        foreach (char freeChar in freeKey)
            //                        {
            //                            if ((charCount[(int)freeChar] -= count) == 0) isFree = true;
            //                        }
            //                        singleFreeCounts.Remove(freeKey);
            //                        doubleFreeCounts.Remove(freeKey);
            //                    }
            //                    if (isFree)
            //                    {
            //                        do
            //                        {
            //                            isFreeKey = false;
            //                            #region 处理独占字符的单词
            //                            for (int i = charCount.Length - 1; i >= 7 && singleFreeCounts.Count != 0; i--)
            //                            {
            //                                if (charCount[i] > 0)
            //                                {
            //                                    removeKey = new list<string>();
            //                                    foreach (string key in singleFreeCounts.Keys)
            //                                    {
            //                                        if ((count = allEndCount[key].Count) == charCount[i] && key.IndexOf((char)i) != -1)
            //                                        {
            //                                            if (CheckDoubleCharCompressReplace(replaceCode, freeChars, allEndCount[key], key, ((char)i).ToString()))
            //                                            {
            //                                                isFreeKey = true;
            //                                                freeKeys.Add(key);
            //                                                freeChars.Add(((char)i).ToString());
            //                                                replaceCode = DoubleCharCompressReplace(replaceCode, allEndCount, key, ((char)i).ToString());
            //                                                removeKey.Add(key);
            //                                                doubleFreeCounts.Remove(key);
            //                                                foreach (char freeChar in key) charCount[(int)freeChar] -= count;
            //                                            }
            //                                            charCount[i] = -1;
            //                                        }
            //                                    }
            //                                    foreach (string key in removeKey) singleFreeCounts.Remove(key);
            //                                }
            //                            }
            //                            #endregion

            //                            #region 为空余字符分配单词
            //                            do
            //                            {
            //                                isFree = false;
            //                                for (int i = charCount.Length - 1; i >= 7 && singleFreeCounts.Count != 0; i--)
            //                                {
            //                                    if (charCount[i] == 0)
            //                                    {
            //                                        freeKey = null;
            //                                        foreach (string key in singleFreeCounts.Keys)
            //                                        {
            //                                            if (freeKey == null || singleFreeCounts[key] > singleFreeCounts[freeKey]) freeKey = key;
            //                                        }
            //                                        if (CheckDoubleCharCompressReplace(replaceCode, freeChars, allEndCount[freeKey], freeKey, ((char)i).ToString()))
            //                                        {
            //                                            isFreeKey = true;
            //                                            count = allEndCount[freeKey].Count;
            //                                            foreach (char freeChar in freeKey)
            //                                            {
            //                                                if ((charCount[(int)freeChar] -= count) == 0) isFree = true;
            //                                            }
            //                                            freeKeys.Add(freeKey);
            //                                            singleFreeCounts.Remove(freeKey);
            //                                            doubleFreeCounts.Remove(freeKey);
            //                                            freeChars.Add(((char)i).ToString());
            //                                            replaceCode = DoubleCharCompressReplace(replaceCode, allEndCount, freeKey, ((char)i).ToString());
            //                                        }
            //                                        charCount[i] = -1;
            //                                    }
            //                                }
            //                            }
            //                            while (isFree && singleFreeCounts.Count != 0);
            //                            #endregion
            //                        }
            //                        while (isFreeKey && singleFreeCounts.Count != 0);
            //                    }
            //                }
            //                while (isFreeDoubleKey && doubleFreeCounts.Count != 0);

            //                #region 压缩代码
            //                stringBuilder newCode = new stringBuilder();
            //                newCode.Add(@"{var a=(""");
            //                count = 0;
            //                for (int i = freeChars.Count - 1; i >= 0; i--)
            //                {
            //                    if (count == 0) count++;
            //                    else newCode.Append(charSplitChar);
            //                    newCode.Add(freeChars[i]);
            //                }
            //                newCode.Append(@""").split('", charSplitChar.ToString(), @"'),A=(""");
            //                count = 0;
            //                for (int i = freeChars.Count - 1; i >= 0; i--)
            //                {
            //                    if (count == 0) count++;
            //                    else newCode.Append(splitChar);
            //                    newCode.Add(freeKeys[i].Replace(@"\", @"\\").Replace(@"""", @"\"""));
            //                }
            //                newCode.Append(@""").split('", splitChar.ToString(), "')");
            //                newCode.Append(@",c=""", replaceCode.Replace(@"\", @"\\").Replace(@"""", @"\"""), @""",i;");
            //                newCode.Add("for(i=0;i<a.length;i++)c=c.split(a[i]).join(A[i]);eval(c);}");
            //                //newCode.append("for(f=i=0;i<a.length;i++)c=c.split(a[i]).join(e+d(++f));for(f=i=0;i<a.length;i++)c=c.split(e+d(++f)).join(A[i]);eval(c);}");
            //                code = newCode.ToString();
            //                #endregion
            //            }
            //        }
            //    }
            //    return code;
            //}
            //#endregion

            //#region 双字符压缩代码替换处理
            ///// <summary>
            ///// 双字符压缩代码替换处理
            ///// </summary>
            ///// <param name="code">源代码</param>
            ///// <param name="allIndex">位置集合</param>
            ///// <param name="key">被替换单词</param>
            ///// <param name="replaceString">替换字符串</param>
            ///// <returns>替换处理后的代码</returns>
            //private static string DoubleCharCompressReplace(string code, Dictionary<string, collection<int>> allIndex, string key, string replaceString)
            //{
            //    int length = key.Length - replaceString.Length;
            //    collection<int> indexs, replaceIndex = new collection<int>();
            //    foreach (int index in allIndex[key]) replaceIndex.AddExpand(index);
            //    foreach (int index in replaceIndex)
            //    {
            //        code = code.Remove(index - key.Length, key.Length).Insert(index - key.Length, replaceString);
            //        foreach (string indexKey in allIndex.Keys)
            //        {
            //            if (indexKey != key)
            //            {
            //                for (int i = (indexs = allIndex[indexKey]).Count - 1; i >= 0 && indexs[i] > index; i--) indexs[i] -= length;
            //            }
            //        }
            //    }
            //    allIndex.Remove(key);
            //    return code;
            //}
            //#endregion

            //#region 检测双字符压缩代码替换处理是否可用
            ///// <summary>
            ///// 检测双字符压缩代码替换处理是否可用
            ///// </summary>
            ///// <param name="code">源代码</param>
            ///// <param name="charKeys">已生成的替换字符串集合</param>
            ///// <param name="indexs">位置集合</param>
            ///// <param name="key">被替换单词</param>
            ///// <param name="replaceString">替换字符串</param>
            ///// <returns>替换处理是否可行</returns>
            //private static bool CheckDoubleCharCompressReplace(string code, list<string> charKeys, collection<int> indexs, string key, string replaceString)
            //{
            //    bool isCheck = true;
            //    for (int i = indexs.Count - 1; i >= 0; i--) code = code.Remove(indexs[i] - key.Length, key.Length).Insert(indexs[i] - key.Length, replaceString);
            //    foreach (string charKey in charKeys)
            //    {
            //        if (charKey.Length > 1 && code.IndexOf(charKey, StringComparison.Ordinal) != -1)
            //        {
            //            isCheck = false; break;
            //        }
            //    }
            //    return isCheck;
            //}
            //#endregion
        }
        /// <summary>
        /// 安装参数
        /// </summary>
        private auto.parameter parameter;
        /// <summary>
        /// HTML模板处理集合
        /// </summary>
        private Dictionary<hashString, htmlJs> htmls;
        /// <summary>
        /// 网站生成配置
        /// </summary>
        private webConfig webConfig;
        /// <summary>
        /// 模板版本号
        /// </summary>
        private string version;
        /// <summary>
        /// 安装入口
        /// </summary>
        /// <param name="parameter">安装参数</param>
        /// <returns>是否安装成功</returns>
        public bool Run(auto.parameter parameter)
        {
            if ((webConfig = parameter.WebConfig) != null && webConfig.IsWebView)
            {
                this.parameter = parameter;
                Type exportPathType = webConfig.ExportPathType;
                if (exportPathType != null)
                {
                    DirectoryInfo viewDirectory = new DirectoryInfo(parameter.ProjectPath + webConfig.ViewJsDirectory + Path.DirectorySeparatorChar);
                    if (viewDirectory.Exists)
                    {
                        string webPathFileName = viewDirectory.fullName() + "webPath";
                        if (!(webConfig.IsExportPathTypeScript ? fastCSharp.ui.code.cSharp.webPath.ts.Default.Run(exportPathType, webPathFileName) : fastCSharp.ui.code.cSharp.webPath.js.Default.Run(exportPathType, webPathFileName)))
                        {
                            error.Message("WEB Path 生成失败");
                        }
                    }
                    else error.Message("没有找到 WEB视图扩展默认目录 " + viewDirectory.FullName);
                }
                if (webConfig.IsCopyScript)
                {
                    string scriptPath = fastCSharp.config.web.Default.FastCSharpTypesciptPath;
                    if (scriptPath == null) error.Message("没有找到js文件路径 " + scriptPath);
                    else copyScript(scriptPath);
                }
                this.version = ((uint)(date.NowSecond - versionTime).TotalSeconds).toHex8().TrimStart('0');
                htmls = dictionary.CreateHashString<htmlJs>();
                if (!htmlJs.Create(this)) return false;
                css();
            }
            return true;
        }
        /// <summary>
        /// 复制脚本文件
        /// </summary>
        /// <param name="fastCSharpScriptPath"></param>
        private void copyScript(string fastCSharpScriptPath)
        {
            string scriptPath = parameter.ProjectPath + @"js\";
            DirectoryInfo fastCSharpPath = new DirectoryInfo(fastCSharpScriptPath);
            copyJs(fastCSharpPath, scriptPath);
            foreach (DirectoryInfo directory in fastCSharpPath.GetDirectories())
            {
                switch (directory.Name)
                {
                    case "ace": copyJs(directory, scriptPath + directory.Name + @"\", new string[] { "load.js", "ace.js" }); break;
                    case "mathJax": copyJs(directory, scriptPath + directory.Name + @"\", new string[] { "load.js", "MathJax.js" }); break;
                    case "highcharts": copyJs(directory, scriptPath + directory.Name + @"\"); break;
                    default: error.Add("未知的js文件夹 " + directory.fullName()); break;
                }
            }
        }
        /// <summary>
        /// 版本号起始时间
        /// </summary>
        private static readonly DateTime versionTime = new DateTime(2016, 4, 22, 0, 3, 16);
        /// <summary>
        /// css背景图片域名替换
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private string cssBackgroundReplace(Match match)
        {
            return @"background" + (match.Groups.Count == 1 ? "-image" : "") + ":url(//" + webConfig.StaticFileDomain + "/images/";
        }
        /// <summary>
        /// css嵌入文件
        /// </summary>
        private Dictionary<hashString, string> includeCss = dictionary.CreateHashString<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string getIncludeCss(string fileName)
        {
            string content;
            hashString fileKey = fileName.toLower();
            if (!includeCss.TryGetValue(fileKey, out content))
            {
                content = File.ReadAllText(fileName, webConfig.Encoding);
                string includeStart = @"/*include:";
                if (content.StartsWith(includeStart, StringComparison.Ordinal))
                {
                    int index = content.IndexOf("*/", includeStart.Length);
                    if (index != -1)
                    {
                        list<string> contents = content.Substring(includeStart.Length, index - includeStart.Length).Split(',')
                            .getList(value => getIncludeCss(parameter.ProjectPath + value + ".css"));
                        contents.Add(content.Substring(index + 2));
                        content = System.String.Join(@"
", contents.ToArray());
                    }
                }
                includeCss.Add(fileKey, content);
            }
            return content;
        }
        /// <summary>
        /// 创建目标css文件
        /// </summary>
        private void css()
        {
            foreach (string pageFileName in Directory.GetFiles(parameter.ProjectPath, "*" + cssPageExtension, SearchOption.AllDirectories))
            {
                string fileName = pageFileName.Substring(0, pageFileName.Length - cssPageExtension.Length) + ".css";
                string content = getIncludeCss(pageFileName)
                    .Replace("__STATICDOMAIN__", webConfig.StaticFileDomain)
                    .Replace("__IMAGEMAIN__", webConfig.ImageDomain);
                //string code = File.ReadAllText(pageFileName, webConfig.Encoding)
                //    .Replace("__STATICDOMAIN__", webConfig.StaticFileDomain)
                //    .Replace("__IMAGEMAIN__", webConfig.ImageDomain);
                content = cssBackgroundRegex.Replace(content, cssBackgroundReplace);
                if (!File.Exists(fileName) || File.ReadAllText(fileName, webConfig.Encoding) != content)
                {
                    File.WriteAllText(fileName, content, webConfig.Encoding);
                }
            }
        }
        /// <summary>
        /// 复制js相关文件
        /// </summary>
        /// <param name="fastCSharpPath">fastCSharp源文件目录</param>
        /// <param name="projectPath">项目文件目录</param>
        private void copyJs(DirectoryInfo fastCSharpPath, string projectPath)
        {
            if (!Directory.Exists(projectPath)) Directory.CreateDirectory(projectPath);
            foreach (FileInfo file in fastCSharpPath.GetFiles())
            {
                if (!file.Extension.EndsWith(".ts", StringComparison.Ordinal)) copyJs(file, projectPath + file.Name);
            }
        }
        /// <summary>
        /// 复制js相关文件
        /// </summary>
        /// <param name="fastCSharpPath">fastCSharp源文件目录</param>
        /// <param name="projectPath">项目文件目录</param>
        /// <param name="fileNames">项目文件目录</param>
        private void copyJs(DirectoryInfo fastCSharpPath, string projectPath, string[] fileNames)
        {
            if (!Directory.Exists(projectPath)) Directory.CreateDirectory(projectPath);
            string path = fastCSharpPath.fullName();
            foreach (string fileName in fileNames)
            {
                FileInfo file = new FileInfo(path + fileName);
                if (file.Exists) copyJs(file, projectPath + file.Name);
            }
        }
        /// <summary>
        /// 复制js相关文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        private void copyJs(FileInfo file, string fileName)
        {
            FileInfo projectFile = new FileInfo(fileName);
            if (projectFile.Exists)
            {
                if (projectFile.LastWriteTimeUtc == file.LastWriteTimeUtc) return;
                projectFile.Attributes &= ~FileAttributes.ReadOnly;
                projectFile.Delete();
            }
            copyJs(file, projectFile);
        }
        /// <summary>
        /// 复制js相关文件
        /// </summary>
        /// <param name="file">fastCSharp源文件</param>
        /// <param name="projectFile">项目文件</param>
        private void copyJs(FileInfo file, FileInfo projectFile)
        {
            if (webConfig.Encoding.CodePage != Encoding.UTF8.CodePage)
            {
                string extension = file.Extension.toLower();
                if (extension == ".ts" || extension == ".js" || extension == ".htm" || extension == ".css")
                {
                    File.WriteAllText(projectFile.FullName, File.ReadAllText(file.FullName, Encoding.UTF8), webConfig.Encoding);
                    projectFile.LastWriteTimeUtc = file.LastWriteTimeUtc;
                    return;
                }
            }
            file.CopyTo(projectFile.FullName);
            new FileInfo(projectFile.FullName).Attributes &= ~FileAttributes.ReadOnly;
        }
    }
}
