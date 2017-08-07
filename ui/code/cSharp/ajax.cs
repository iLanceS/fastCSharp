using System;
using fastCSharp.net.tcp.http;
using System.Collections.Generic;
using fastCSharp.threading;
using fastCSharp.code.cSharp;
using System.IO;

namespace fastCSharp.code
{
    /// <summary>
    /// AJAX调用配置
    /// </summary>
    internal sealed partial class ajax
    {
        /// <summary>
        /// 默认空AJAX调用配置
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.ajax Null = new fastCSharp.code.cSharp.ajax();

        /// <summary>
        /// AJAX调用代码生成
        /// </summary
        [auto(Name = "AJAX调用", DependType = typeof(webView.cSharp), IsAuto = true)]
        internal partial class cSharp : webView.cSharp<fastCSharp.code.cSharp.ajax>, IAuto
        {
            /// <summary>
            /// 方法索引信息
            /// </summary>
            public sealed class methodIndex : asynchronousMethod
            {
                /// <summary>
                /// 获取该方法的类型
                /// </summary>
                public memberType WebViewMethodType;
                /// <summary>
                /// 方法索引
                /// </summary>
                public int MethodIndex;
                /// <summary>
                /// 方法索引名称
                /// </summary>
                public string MethodIndexName
                {
                    get
                    {
                        return "_m" + MethodIndex.ToString();
                    }
                }
                /// <summary>
                /// 异步处理类索引名称
                /// </summary>
                public string AsyncIndexName
                {
                    get
                    {
                        return "_a" + MethodIndex.toString() + Method.GenericParameterName;
                    }
                }
                /// <summary>
                /// 类型AJAX调用配置
                /// </summary>
                public fastCSharp.code.cSharp.ajax TypeAttribute;
                /// <summary>
                /// 类型调用名称
                /// </summary>
                public string TypeCallName;
                /// <summary>
                /// AJAX调用配置
                /// </summary>
                private fastCSharp.code.cSharp.ajax attribute;
                /// <summary>
                /// AJAX调用配置
                /// </summary>
                public fastCSharp.code.cSharp.ajax Attribute
                {
                    get
                    {
                        if (attribute == null)
                        {
                            attribute = (MemberIndex ?? Method).GetSetupAttribute<fastCSharp.code.cSharp.ajax>(false, false);
                            if (attribute == null) attribute = ajax.Null;
                        }
                        return attribute;
                    }
                }
                /// <summary>
                /// 是否忽略大小写
                /// </summary>
                public bool IgnoreCase;
                /// <summary>
                /// 调用名称
                /// </summary>
                private string callName;
                /// <summary>
                /// 调用名称
                /// </summary>
                public string CallName
                {
                    get
                    {
                        if (callName == null)
                        {
                            if (Attribute.FullName != null) callName = IgnoreCase ? attribute.FullName.ToLower() : attribute.FullName;
                            else
                            {
                                string name = attribute.MethodName ?? Method.MethodName;
                                if (name.Length == 0) callName = IgnoreCase ? TypeCallName.ToLower() : TypeCallName;
                                else
                                {
                                    callName = TypeCallName + "." + name;
                                    if (IgnoreCase) callName = callName.toLower();
                                }
                            }
                        }
                        return callName;
                    }
                }
                /// <summary>
                /// 默认AJAX调用配置
                /// </summary>
                public fastCSharp.code.cSharp.ajax DefaultAttribute
                {
                    get
                    {
                        return Attribute == ajax.Null ? TypeAttribute : attribute;
                    }
                }
                /// <summary>
                /// 最大接收数据字节数
                /// </summary>
                public int MaxPostDataSize
                {
                    get
                    {
                        return DefaultAttribute.MaxPostDataSize << 20;
                    }
                }
                /// <summary>
                /// 内存流最大字节数
                /// </summary>
                public int MaxMemoryStreamSize
                {
                    get
                    {
                        return DefaultAttribute.MaxMemoryStreamSize << 10;
                    }
                }
                /// <summary>
                /// 是否存在输入参数
                /// </summary>
                public bool IsParameter
                {
                    get
                    {
                        return MethodParameters.Length != 0 || MethodIsReturn;
                    }
                }
                /// <summary>
                /// 参数名称
                /// </summary>
                public string ParameterTypeName
                {
                    get
                    {
                        return "_p" + MethodIndex.toString();
                    }
                }
                /// <summary>
                /// 是否存在输入参数
                /// </summary>
                public bool IsInputParameter
                {
                    get
                    {
                        foreach (parameterInfo parameter in MethodParameters)
                        {
                            if (!parameter.IsOut) return true;
                        }
                        return false;
                    }
                }
                /// <summary>
                /// 是否存在输出参数
                /// </summary>
                public bool IsOutputParameter
                {
                    get
                    {
                        return Method.OutputParameters.Length != 0 || MethodIsReturn;
                    }
                }
                /// <summary>
                /// 异步回调是否检测成功状态
                /// </summary>
                protected override bool isAsynchronousFunc { get { return false; } }
            }
            /// <summary>
            /// web视图AJAX调用索引信息
            /// </summary>
            public sealed class viewMethodIndex
            {
                /// <summary>
                /// 获取改方法的类型
                /// </summary>
                public memberType WebViewMethodType;
                /// <summary>
                /// WEB视图配置
                /// </summary>
                public fastCSharp.code.cSharp.webView Attribute;
                /// <summary>
                /// 方法索引
                /// </summary>
                public int MethodIndex;
                /// <summary>
                /// 方法索引名称
                /// </summary>
                public string ViewMethodIndexName
                {
                    get
                    {
                        return "_v" + MethodIndex.ToString();
                    }
                }
                /// <summary>
                /// 调用名称
                /// </summary>
                public string CallName;
                /// <summary>
                /// 最大接收数据字节数
                /// </summary>
                public int MaxPostDataSize
                {
                    get
                    {
                        return Attribute.MaxPostDataSize << 20;
                    }
                }
                /// <summary>
                /// 内存流最大字节数
                /// </summary>
                public int MaxMemoryStreamSize
                {
                    get
                    {
                        return Attribute.MaxMemoryStreamSize << 10;
                    }
                }
            }
            /// <summary>
            /// AJAX API代码生成
            /// </summary>
            private static readonly ts ts = new ts();
            /// <summary>
            /// AJAX函数
            /// </summary>
            private list<methodIndex> methods = new list<methodIndex>();
            /// <summary>
            /// AJAX函数
            /// </summary>
            public methodIndex[] Methods;
            /// <summary>
            /// web视图AJAX调用
            /// </summary>
            public viewMethodIndex[] ViewMethods;
            /// <summary>
            /// AJAX调用名称
            /// </summary>
            public string AjaxName
            {
                get { return fastCSharp.config.web.Default.AjaxWebCallName; }
            }
            /// <summary>
            /// 是否存在公用错误处理函数
            /// </summary>
            public bool IsPubError;
            /// <summary>
            /// AJAX函数数量
            /// </summary>
            public int MethodCount;
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                string typeCallName = type.FullName;
                if (typeCallName.StartsWith(AutoParameter.DefaultNamespace, StringComparison.Ordinal) && typeCallName[AutoParameter.DefaultNamespace.Length] == '.')
                {
                    typeCallName = typeCallName.Substring(AutoParameter.DefaultNamespace.Length + 1);
                }
                if (typeCallName.StartsWith("ajax.", StringComparison.Ordinal)) typeCallName = typeCallName.Substring("ajax.".Length);

                int methodIndex = methods.Count;
                methodIndex[] methodIndexs = code.methodInfo.GetMethods<fastCSharp.code.cSharp.ajax>(type, code.memberFilters.PublicInstance, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute)
                    .getArray(value => new methodIndex
                    {
                        Method = value,
                        MethodIndex = methodIndex++,
                        WebViewMethodType = type,
                        TypeAttribute = Attribute,
                        TypeCallName = typeCallName,
                        IgnoreCase = AutoParameter.WebConfig.IgnoreCase
                    });
                if (methodIndexs.Length != 0)
                {
                    methods.Add(methodIndexs);
                    if (Attribute.IsExportTypeScript) ts.Create(AutoParameter, type, methodIndexs);
                }
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected override void onCreated()
            {
                int methodIndex = methods.Count;
                ViewMethods = new webView.cSharp { AutoParameter = AutoParameter }.GetTypeAttributes()
                    .getFind(value => value.Value.IsAjax)
                    .GetArray(value => new viewMethodIndex
                    {
                        MethodIndex = methodIndex++,
                        WebViewMethodType = value.Key,
                        Attribute = value.Value,
                        CallName = new webView.cSharp.viewType { type = value.Key, Attribute = value.Value, DefaultNamespace = AutoParameter.DefaultNamespace + ".", IgnoreCase = AutoParameter.WebConfig.IgnoreCase }.CallName
                    });
                if (methodIndex != 0)
                {
                    subArray<KeyValuePair<string, int>> names = (Methods = methods.ToArray()).getArray(value => value.CallName)
                        .concat(ViewMethods.getArray(value => value.CallName))
                        .groupCount(value => value)
                        .getFind(value => value.Value != 1);
                    if (names.Count == 0)
                    {
                        IsPubError = Methods.any(method => method.CallName == fastCSharp.code.cSharp.ajax.PubErrorCallName);
                        MethodCount = Methods.Length + ViewMethods.Length + (IsPubError ? 0 : 1);
                        _code_.Empty();
                        create(false);
                        fastCSharp.code.coder.Add("namespace " + AutoParameter.DefaultNamespace + @"
{
" + _code_.ToString() + @"
}");
                        ts.OnCreated();
                    }
                    else
                    {
                        error.Add(@"AJAX调用名称冲突：
" + names.joinString(@"
", value => value.Key + "[" + value.Value.toString() + "]"));
                    }
                }
            }
        }
        /// <summary>
        /// AJAX API代码生成
        /// </summary>
        [auto(Name = "AJAX API", Language = auto.language.TypeScript)]
        internal sealed partial class ts : cSharp
        {
            /// <summary>
            /// API 命名空间
            /// </summary>
            public const string fastCSharpAPI = "fastCSharpAPI";
            /// <summary>
            /// 代码
            /// </summary>
            private static stringBuilder code = new stringBuilder();
            /// <summary>
            /// 命名空间
            /// </summary>
            public string Namespace;
            /// <summary>
            /// 创建代码
            /// </summary>
            /// <param name="type"></param>
            /// <param name="methodIndexs"></param>
            public void Create(auto.parameter parameter, memberType type, methodIndex[] methodIndexs)
            {
                AutoParameter = parameter;
                this.type = type;
                Methods = methodIndexs;

                Namespace = type.Type.Namespace;
                if (Namespace == AutoParameter.DefaultNamespace) Namespace = fastCSharpAPI;
                else Namespace = Namespace.StartsWith(AutoParameter.DefaultNamespace, StringComparison.Ordinal) && Namespace[AutoParameter.DefaultNamespace.Length] == '.' ? fastCSharpAPI + Namespace.Substring(AutoParameter.DefaultNamespace.Length) : Namespace;

                _code_.Empty();
                create(false);
                code.Add(_code_);
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            public void OnCreated()
            {
                string webViewCode = webView.ts.Code;
                if (code.Count != 0 || webViewCode != null)
                {
                    string apiCode = @"//本文件由程序自动生成,请不要自行修改
" + code.ToString() + (webViewCode == null ? null : @"
" + webViewCode);
                    code.Empty();
                    string fileName = AutoParameter.ProjectPath + @"viewJs\api.ts";
                    if (fastCSharp.code.coder.WriteFileSuffix(fileName, apiCode)) fastCSharp.code.error.Message(fileName + " 被修改");
                }
            }
        }
    }
}
