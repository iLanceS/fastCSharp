using System;
using System.Collections.Generic;
using System.IO;
using fastCSharp.reflection;
using fastCSharp.io;

namespace fastCSharp.code
{
    /// <summary>
    /// 模板代码生成器
    /// </summary>
    internal sealed class coder : template<treeBuilder.node>
    {
        /// <summary>
        /// 模板文件路径
        /// </summary>
        private static readonly string templatePath = (@"ui\code\cSharp\template\").pathSeparator();
        /// <summary>
        /// 模板命令类型
        /// </summary>
        internal enum command
        {
            /// <summary>
            /// 输出绑定的数据
            /// </summary>
            AT,
            /// <summary>
            /// 子代码段
            /// </summary>
            PUSH,
            /// <summary>
            /// 循环
            /// </summary>
            LOOP,
            /// <summary>
            /// 循环=LOOP
            /// </summary>
            FOR,
            /// <summary>
            /// 屏蔽代码段输出
            /// </summary>
            NOTE,
            /// <summary>
            /// 绑定的数据为true非0非null时输出代码
            /// </summary>
            IF,
            /// <summary>
            /// 绑定的数据为false或者0或者null时输出代码
            /// </summary>
            NOT,
            /// <summary>
            /// 用于标识一个子段模板，可以被别的模板引用
            /// </summary>
            NAME,
            /// <summary>
            /// 引用NAME标识一个子段模板
            /// </summary>
            FROMNAME,
            /// <summary>
            /// 用于标识一个子段程序代码，用于代码的分类输出
            /// </summary>
            PART,
        }
        /// <summary>
        /// 声明与警告+文件头
        /// </summary>
        public const string WarningCode = @"//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
";
        /// <summary>
        /// 文件结束
        /// </summary>
        public const string FileEndCode = @"
#endif";
        static coder()
        {
            codes = new stringBuilder[Enum.GetMaxValue<auto.language>(-1) + 1];
            for (int index = codes.Length; index != 0; codes[--index] = new stringBuilder()) ;
        }
        /// <summary>
        /// 已经生成的代码
        /// </summary>
        private static readonly stringBuilder[] codes;
        /// <summary>
        /// 没有依赖的记忆代码
        /// </summary>
        private static stringBuilder rememberCodes = new stringBuilder();
        ///// <summary>
        ///// 是否存在生成代码
        ///// </summary>
        //public static bool IsCode
        //{
        //    get { return codes.Count != 0 || rememberCodes.Count != 0; }
        //}
        /// <summary>
        /// 已经生成代码的类型
        /// </summary>
        private struct type : IEquatable<type>
        {
            /// <summary>
            /// 模板类型
            /// </summary>
            public Type TemplateType;
            /// <summary>
            /// 当前生成代码的应用类型
            /// </summary>
            public Type Type;
            /// <summary>
            /// 判断生成代码类型是否相等
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(type other)
            {
                return TemplateType == other.TemplateType && Type == other.Type;
            }
            /// <summary>
            /// 判断生成代码类型是否相等
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return Equals((type)obj);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return TemplateType.GetHashCode() ^ Type.GetHashCode();
            }
        }
        /// <summary>
        /// 已经生成代码的类型
        /// </summary>
        private static HashSet<type> codeTypes = hashSet<type>.Create();
        /// <summary>
        /// CSharp代码树节点缓存
        /// </summary>
        private static Dictionary<string, treeBuilder.node> nodeCache = dictionary.CreateOnly<string, treeBuilder.node>();
        /// <summary>
        /// 安装参数
        /// </summary>
        private auto.parameter parameter;
        /// <summary>
        /// 模板文件扩展名
        /// </summary>
        private string extensionName;
        /// <summary>
        /// CSharp代码生成器
        /// </summary>
        /// <param name="parameter">安装参数</param>
        /// <param name="type">模板数据视图</param>
        /// <param name="language">语言</param>
        public coder(auto.parameter parameter, Type type, auto.language language)
            : base(type, error.Add, error.Message)
        {
            this.parameter = parameter;
            extensionName = "." + Enum<auto.language, auto.languageAttribute>.Array((int)(byte)language).ExtensionName;
            creators[command.NOTE.ToString()] = note;
            creators[command.LOOP.ToString()] = creators[command.FOR.ToString()] = loop;
            creators[command.AT.ToString()] = at;
            creators[command.PUSH.ToString()] = push;
            creators[command.IF.ToString()] = ifThen;
            creators[command.NOT.ToString()] = not;
            creators[command.NAME.ToString()] = name;
            creators[command.FROMNAME.ToString()] = fromName;
            creators[command.PART.ToString()] = part;
        }
        /// <summary>
        /// 根据类型名称获取子段模板
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="name">子段模板名称</param>
        /// <returns>子段模板</returns>
        protected override treeBuilder.node fromNameNode(string typeName, string name)
        {
            treeBuilder.node node = getNode(typeName);
            if (node != null)
            {
                node = node.GetFirstNodeByTag(command.NAME, name);
                if (node == null) error.Add("模板文件 " + getTemplateFileName(typeName) + " 未找到NAME " + name);
            }
            return node;
        }
        /// <summary>
        /// 添加代码树节点
        /// </summary>
        /// <param name="node">代码树节点</param>
        private void skinEnd(treeBuilder.node node)
        {
            skin(node);
            pushCode(null);
        }
        /// <summary>
        /// 创建CSharp代码生成器
        /// </summary>
        /// <param name="type">模板数据视图</param>
        /// <param name="auto">安装属性</param>
        /// <returns>生成器代码</returns>
        private string createClass(Type type, auto auto)
        {
            coder code = new coder(parameter, type, auto.Language);
            code.skin(getNode(auto.GetFileName(type)));
            return code.partCodes["CLASS"];
        }
        /// <summary>
        /// 根据类型名称获取模板文件名
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>模板文件名</returns>
        private string getTemplateFileName(string typeName)
        {
            return new DirectoryInfo(parameter.ProjectPath).Parent.fullName() + templatePath + typeName + extensionName;
        }
        /// <summary>
        /// 根据类型名称获取CSharp代码树节点
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>CSharp代码树节点</returns>
        private treeBuilder.node getNode(string fileName)
        {
            treeBuilder.node node;
            if (!nodeCache.TryGetValue(fileName + extensionName, out node))
            {
                fileName = getTemplateFileName(fileName);
                if (File.Exists(fileName))
                {
                    nodeCache.Add(fileName, node = new treeBuilder().create(File.ReadAllText(fileName)));
                }
                else error.Add("未找到模板文件 " + fileName);
            }
            return node;
        }
        /// <summary>
        /// 添加代码
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="language">代码生成语言</param>
        public static void Add(string code, auto.language language = auto.language.CSharp)
        {
            codes[(int)(byte)language].Add(code);
        }
        /// <summary>
        /// 添加代码
        /// </summary>
        /// <param name="cSharperType">模板类型</param>
        /// <param name="type">实例类型</param>
        /// <returns>锁定是否成功</returns>
        public static bool Add(Type cSharperType, Type type)
        {
            if (codeTypes.Contains(new coder.type { TemplateType = cSharperType, Type = type })) return false;
            codeTypes.Add(new coder.type { TemplateType = cSharperType, Type = type });
            return true;
        }
        /// <summary>
        /// 添加没有依赖的记忆代码
        /// </summary>
        /// <param name="code">没有依赖的记忆代码</param>
        public static void AddRemember(string code)
        {
            rememberCodes.Add(code);
        }
        /// <summary>
        /// 输出代码
        /// </summary>
        public static void Output(auto.parameter parameter)
        {
            stringBuilder[] builders = new stringBuilder[codes.Length];
            for (int index = codes.Length; index != 0; )
            {
                stringBuilder builder = codes[--index];
                if (builder.Count != 0)
                {
                    builders[index] = builder;
                    codes[index] = new stringBuilder();
                }
                auto.language language = (auto.language)(byte)index;
                switch (language)
                {
                    case auto.language.JavaScript:
                    case auto.language.TypeScript:
                        if (builders[index] != null) error.Add("生成了未知的 " + language + " 代码。");
                        break;
                }
            }
            stringBuilder rememberCodeBuilder = null;
            if (rememberCodes.Count != 0)
            {
                rememberCodeBuilder = rememberCodes;
                rememberCodes = new stringBuilder();
            }
            codeTypes.Clear();
            error.ThrowError();
            string message = string.Empty;
            for (int index = builders.Length; index != 0;)
            {
                stringBuilder builder = builders[--index];
                if (builder != null)
                {
                    switch (index)
                    {
                        case (int)auto.language.CSharp:
                            string code = builder.ToString(), fastCSharpFileName = null, rememberFileName = null;
                            bool isFastCSharp = false, isRemember = false;
                            if (code.length() != 0)
                            {
                                string fileName = parameter.ProjectPath + (fastCSharpFileName = "{" + parameter.DefaultNamespace + "}." + pub.fastCSharp + ".cs");
                                if (WriteFile(fileName, WarningCode + code + FileEndCode))
                                {
                                    isFastCSharp = true;
                                    message = fileName + " 被修改";
                                }
                            }
                            if (rememberCodeBuilder != null && (code = rememberCodeBuilder.ToString()).length() != 0)
                            {
                                string fileName = parameter.ProjectPath + (rememberFileName = "{" + parameter.DefaultNamespace + "}.remember." + pub.fastCSharp + ".cs");
                                if (WriteFile(fileName, WarningCode + code + FileEndCode))
                                {
                                    isRemember = true;
                                    message += @"
" + fileName + " 被修改";
                                }
                            }
                            if (parameter.IsFastCSharp && (isFastCSharp | isRemember))
                            {
                                string projectFile = parameter.AssemblyPath + parameter.ProjectName + ".csproj";
                                if (File.Exists(projectFile))
                                {
                                    string projectXml = File.ReadAllText(projectFile, System.Text.Encoding.UTF8);
                                    if (isFastCSharp) fastCSharpFileName = @"<Compile Include=""" + fastCSharpFileName + @""" />";
                                    if(isRemember) rememberFileName = @"<Compile Include=""" + rememberFileName + @""" />";
                                    int fileIndex;
                                    if (isFastCSharp && (fileIndex = projectXml.IndexOf(fastCSharpFileName)) != -1)
                                    {
                                        if (isRemember && projectXml.IndexOf(rememberFileName) == -1)
                                        {
                                            projectXml = projectXml.Insert(fileIndex + fastCSharpFileName.Length, @"
    " + rememberFileName);
                                            MoveFile(projectFile, projectXml);
                                        }
                                        break;
                                    }
                                    if (isRemember && (fileIndex = projectXml.IndexOf(rememberFileName)) != -1)
                                    {
                                        if (isFastCSharp && projectXml.IndexOf(fastCSharpFileName) == -1)
                                        {
                                            projectXml = projectXml.Insert(fileIndex + rememberFileName.Length, @"
    " + fastCSharpFileName);
                                            MoveFile(projectFile, projectXml);
                                        }
                                        break;
                                    }
                                    string csFileName = @".cs"" />
";
                                    if ((fileIndex = projectXml.IndexOf(csFileName)) != -1)
                                    {
                                        if (isFastCSharp)
                                        {
                                            fastCSharpFileName += @"
    ";
                                            if (isRemember)
                                            {
                                                fastCSharpFileName += rememberFileName + @"
    ";
                                            }
                                        }
                                        else fastCSharpFileName = rememberFileName + @"
    ";
                                        projectXml = projectXml.Insert(fileIndex + csFileName.Length, fastCSharpFileName);
                                        MoveFile(projectFile, projectXml);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            if (message.Length != 0) fastCSharp.log.Default.ThrowReal(message, new System.Diagnostics.StackFrame(), false);
        }
        /// <summary>
        /// 输出代码
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="content">文件内容</param>
        /// <returns>是否写入新内容</returns>
        public static bool WriteFile(string fileName, string content)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    if (File.ReadAllText(fileName) != content) return MoveFile(fileName, content);
                }
                else
                {
                    File.WriteAllText(fileName, content);
                    return true;
                }
            }
            catch (Exception error)
            {
                log.Default.ThrowReal(error, "文件创建失败 : " + fileName, false);
            }
            return false;
        }
        /// <summary>
        /// 输出代码
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="content">文件内容</param>
        /// <returns>是否写入新内容</returns>
        public static bool WriteFileSuffix(string fileName, string content)
        {
            try
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    if (File.ReadAllText(fileName = file.FullName) != content)
                    {
                        string bakName = file.Directory.fullName() + fastCSharp.io.file.BakPrefix + date.Now.ToString("yyyyMMdd-HHmmss") + "_" + file.Name + "." + ((uint)random.Default.Next()).toString();
                        if (File.Exists(bakName)) File.Delete(bakName);
                        File.Move(fileName, bakName);
                        File.WriteAllText(fileName, content);
                        return true;
                    }
                }
                else
                {
                    DirectoryInfo directory = file.Directory;
                    if (!directory.Exists) directory.Create();
                    File.WriteAllText(fileName, content);
                    return true;
                }
            }
            catch (Exception error)
            {
                log.Default.ThrowReal(error, "文件创建失败 : " + fileName, false);
            }
            return false;
        }
        /// <summary>
        /// 输出代码
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="content">文件内容</param>
        /// <returns>是否写入新内容</returns>
        public static bool MoveFile(string fileName, string content)
        {
            try
            {
                fastCSharp.io.file.MoveBak(fileName);
                File.WriteAllText(fileName, content);
                return true;
            }
            catch (Exception error)
            {
                log.Default.ThrowReal(error, "文件创建失败 : " + fileName, false);
            }
            return false;
        }

        /// <summary>
        /// CSharp代码生成数据视图生成
        /// </summary>
        [auto(Name = "C#", IsAuto = true, Template = null, IsTemplate = false)]
        internal sealed class cSharper : IAuto
        {
            /// <summary>
            /// 类定义生成
            /// </summary>
            private sealed class definition
            {
                /// <summary>
                /// 类型
                /// </summary>
                public Type Type;
                /// <summary>
                /// 安装属性
                /// </summary>
                public auto Auto;
                /// <summary>
                /// 安装参数
                /// </summary>
                public auto.parameter Parameter;
                /// <summary>
                /// 类定义生成
                /// </summary>
                private fastCSharp.code.cSharper.definition.cSharp typeDefinition;
                /// <summary>
                /// 模板代码生成器
                /// </summary>
                private coder coder;
                /// <summary>
                /// 模板代码
                /// </summary>
                private stringBuilder codeBuilder;
                /// <summary>
                /// 生成类定义字符串
                /// </summary>
                /// <returns>类定义字符串</returns>
                public override string ToString()
                {
                    typeDefinition = new fastCSharp.code.cSharper.definition.cSharp(Type, true, true);
                    (coder = new coder(Parameter, Type, Auto.Language)).skinEnd(coder.getNode(Auto.GetFileName(Type)));
                    (codeBuilder = new stringBuilder()).Append(@"
", typeDefinition.Start, @"
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name=""isOut"">是否输出代码</param>
        protected override void create(bool _isOut_)
        {
            if (outStart(fastCSharp.code.auto.language.", Auto.Language.ToString(), @", _isOut_))
            {
                ");
                    switch (Auto.Language)
                    {
                        case auto.language.JavaScript:
                        case auto.language.TypeScript:
                            return javaScript();
                        default: return cSharp();
                    }
                }
                /// <summary>
                /// 模板结束
                /// </summary>
                /// <returns></returns>
                private string end()
                {
                    codeBuilder.Append(@"
                if (_isOut_) outEnd();
            }
        }", typeDefinition.End);
                    return codeBuilder.ToString();
                }
                /// <summary>
                /// 生成C#模板代码
                /// </summary>
                /// <returns></returns>
                private string cSharp()
                {
                    codeBuilder.Append(coder.partCodes["CLASS"]);
                    return end();
                }
                /// <summary>
                /// 生成JavaScript模板代码
                /// </summary>
                /// <returns></returns>
                private string javaScript()
                {
                    codeBuilder.Add(coder.code);
                    return end();
                }
            }
            /// <summary>
            /// 安装入口
            /// </summary>
            /// <param name="parameter">安装参数</param>
            /// <returns>是否安装成功</returns>
            public bool Run(auto.parameter parameter)
            {
                if (parameter != null)
                {
                    if (parameter.IsFastCSharp && parameter.ProjectName == pub.fastCSharp)
                    {
                        subArray<definition> definitions = ui.CurrentAssembly.GetTypes().getArray(type => new definition { Type = type, Auto = type.customAttribute<auto>(), Parameter = parameter })
                            .getFind(type => type.Auto != null && type.Auto.IsTemplate)// && type.Auto.DependType == typeof(cSharper)
                            .Sort((left, right) => string.CompareOrdinal(left.Type.FullName, right.Type.FullName));
                        subArray<string> codes = new subArray<string>(definitions.Count);
                        foreach (definition definition in definitions)
                        {
                            codes.Add(definition.ToString());
                            if (error.IsError) return false;
                        }
                        string fileName = new DirectoryInfo(parameter.ProjectPath).Parent.fullName() + @"ui\{" + pub.fastCSharp + "}.cSharper.cs";
                        if (WriteFile(fileName, WarningCode + string.Concat(codes) + FileEndCode))
                        {
                            error.Add(fileName + " 被修改");
                            throw new Exception();
                        }
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
