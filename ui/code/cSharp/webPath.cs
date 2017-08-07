using System;
using System.IO;
using System.Reflection;
using fastCSharp.code;
using fastCSharp.reflection;

namespace fastCSharp.ui.code.cSharp
{
    /// <summary>
    /// WEB Path 调用配置
    /// </summary>
    internal sealed partial class webPath
    {
        /// <summary>
        /// Path 成员
        /// </summary>
        internal struct pathMember
        {
            /// <summary>
            /// 成员信息
            /// </summary>
            public fastCSharp.code.memberInfo Member;
            /// <summary>
            /// Path
            /// </summary>
            public string Path;
            /// <summary>
            /// 其他查询前缀
            /// </summary>
            public string OtherQuery;
            /// <summary>
            /// 查询名称
            /// </summary>
            public string QueryName;
            /// <summary>
            /// 是否 Id 传参
            /// </summary>
            public bool IsIdentity;
            /// <summary>
            /// 是否 #! 查询
            /// </summary>
            public bool IsHash;
        }
        /// <summary>
        /// WEB Path 代码生成
        /// </summary>
        internal abstract class cSharp<cSharpType> : member<fastCSharp.code.cSharp.webPath>, IAuto
            where cSharpType : cSharp<cSharpType>, new()
        {
            /// <summary>
            /// 命名空间
            /// </summary>
            public const string fastCSharpPath = "fastCSharpPath";
            /// <summary>
            /// WEB Path 代码生成
            /// </summary>
            internal static readonly cSharpType Default = new cSharpType();
            /// <summary>
            /// 代码
            /// </summary>
            protected stringBuilder code = new stringBuilder();
            /// <summary>
            /// 输出文件扩展名称
            /// </summary>
            protected abstract string outputFileExtensionName { get; }
            /// <summary>
            /// 输出文件名称
            /// </summary>
            private string outputFileName;
            /// <summary>
            /// Path 成员
            /// </summary>
            internal subArray<pathMember> pathMembers;
            /// <summary>
            /// 安装入口
            /// </summary>
            /// <param name="exportPathType">导出引导类型</param>
            /// <returns>是否安装成功</returns>
            public bool Run(Type exportPathType, string outputFileName)
            {
                fastCSharp.code.cSharp.webPath exportWebPath = exportPathType.customAttribute<fastCSharp.code.cSharp.webPath>(false);
                if (exportWebPath == null) error.Message("没有找到 path 导出信息 " + exportPathType.fullName());
                else if (exportWebPath.Flag == 0) error.Message("缺少导出二进制位标识 " + exportPathType.fullName());
                else
                {
                    foreach (Type type in exportPathType.Assembly.GetTypes())
                    {
                        fastCSharp.code.cSharp.webPath webPath = type.customAttribute<fastCSharp.code.cSharp.webPath>(false);
                        if (webPath != null && (webPath.Flag & exportWebPath.Flag) == exportWebPath.Flag)
                        {
                            this.type = type;
                            Attribute = webPath;
                            nextCreate();
                        }
                    }
                    this.outputFileName = outputFileName + outputFileExtensionName;
                    onCreated();
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                pathMembers.Clear();
                object pathValue = typeof(fastCSharp.emit.constructor<>).MakeGenericType(type).GetMethod("Default", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, nullValue<Type>.Array, null).Invoke(null, null);
                string queryName = (Attribute.QueryName ?? (type.Type.Name + "Id")), query;
                FieldInfo idField = type.Type.GetField("Id", BindingFlags.Instance | BindingFlags.Public);
                if (idField == null || idField.FieldType != typeof(int)) query = queryName + "=0";
                else
                {
                    idField.SetValue(pathValue, 1);
                    query = queryName + "=1";
                }
                foreach (fastCSharp.code.memberInfo member in fastCSharp.code.memberInfo.GetMembers<fastCSharp.code.ignoreMember>(type, Attribute.MemberFilter, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute))
                {
                    if (member.Type == typeof(string))
                    {
                        string url = (string)(member.Member as PropertyInfo).GetValue(pathValue, null);
                        if (url.EndsWith(query, StringComparison.Ordinal))
                        {
                            pathMembers.Add(new pathMember { Member = member, Path = url.Substring(0, url.Length - query.Length), QueryName = queryName, IsHash = false, IsIdentity = true });
                        }
                        else pathMembers.Add(new pathMember { Member = member, Path = url });
                    }
                    else if (member.Type == typeof(fastCSharp.code.cSharp.webView.hashUrl))
                    {
                        fastCSharp.code.cSharp.webView.hashUrl url = (fastCSharp.code.cSharp.webView.hashUrl)(member.Member as PropertyInfo).GetValue(pathValue, null);
                        if (url.Query == query) pathMembers.Add(new pathMember { Member = member, Path = url.Path, QueryName = queryName, IsHash = true, IsIdentity = true });
                        else if (url.Query.EndsWith(query, StringComparison.Ordinal))
                        {
                            pathMembers.Add(new pathMember { Member = member, Path = url.Path, OtherQuery = url.Query.Substring(0, url.Query.Length - query.Length), QueryName = queryName, IsHash = true, IsIdentity = true });
                        }
                    }
                }
                if (pathMembers.Count != 0)
                {
                    _code_.Empty();
                    create(false);
                    code.Add(_code_);
                }
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected override void onCreated()
            {
                string webPathCode = @"//本文件由程序自动生成,请不要自行修改
" + onCreatedCode;
                code.Empty();
                if (fastCSharp.code.coder.WriteFileSuffix(outputFileName, webPathCode)) fastCSharp.code.error.Message(outputFileName + " 被修改");
            }
            /// <summary>
            /// 安装完成处理代码
            /// </summary>
            protected abstract string onCreatedCode { get; }
        }
        /// <summary>
        /// WEB Path JavaScript 代码生成
        /// </summary>
        [auto(Name = "WEB Path JavaScript", Language = auto.language.JavaScript)]
        internal sealed partial class js : cSharp<js>
        {
            /// <summary>
            /// 输出文件扩展名称
            /// </summary>
            protected override string outputFileExtensionName { get { return ".js"; } }
            /// <summary>
            /// 命名空间
            /// </summary>
            public string Namespace
            {
                get { return fastCSharpPath; }
            }
            /// <summary>
            /// 安装完成处理代码
            /// </summary>
            protected override string onCreatedCode
            {
                get
                {
                    return "var " + fastCSharpPath + @"={
" + code.ToString() + @"_:0};";
                }
            }
        }
        /// <summary>
        /// WEB Path TypeScript 代码生成
        /// </summary>
        [auto(Name = "WEB Path TypeScript", Language = auto.language.TypeScript)]
        internal sealed partial class ts : cSharp<ts>
        {
            /// <summary>
            /// 输出文件扩展名称
            /// </summary>
            protected override string outputFileExtensionName { get { return ".ts"; } }
            /// <summary>
            /// 安装完成处理代码
            /// </summary>
            protected override string onCreatedCode
            {
                get
                {
                    return @"namespace fastCSharpPath {
" + code.ToString() + @"}";
                }
            }
        }
    }
}
