using System;
using System.Threading;
using fastCSharp.threading;
using fastCSharp.reflection;

namespace fastCSharp.code
{
    /// <summary>
    /// web调用配置
    /// </summary>
    internal sealed partial class webCall
    {
        /// <summary>
        /// 默认空WEB调用配置
        /// </summary>
        internal static readonly fastCSharp.code.cSharp.webCall Null = new fastCSharp.code.cSharp.webCall();
        /// <summary>
        /// web调用代码生成
        /// </summary
        [auto(Name = "web调用", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : webView.cSharp<fastCSharp.code.cSharp.webCall>, IAuto
        {
            /// <summary>
            /// 方法索引信息
            /// </summary>
            public sealed class methodIndex : asynchronousMethod
            {
                /// <summary>
                /// 获取改方法的类型
                /// </summary>
                public memberType WebCallMethodType;
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
                /// WEB调用代理名称
                /// </summary>
                public string MethodCallName
                {
                    get { return "_c" + MethodIndex.ToString(); }
                }
                /// <summary>
                /// 类型WEB调用配置
                /// </summary>
                public fastCSharp.code.cSharp.webCall TypeAttribute;
                /// <summary>
                /// 类型调用名称
                /// </summary>
                public string TypeCallName;
                /// <summary>
                /// WEB调用配置
                /// </summary>
                private fastCSharp.code.cSharp.webCall attribute;
                /// <summary>
                /// WEB调用配置
                /// </summary>
                public fastCSharp.code.cSharp.webCall Attribute
                {
                    get
                    {
                        if (attribute == null)
                        {
                            attribute = (MemberIndex ?? Method).GetSetupAttribute<fastCSharp.code.cSharp.webCall>(false, false);
                            if (attribute == null) attribute = webCall.Null;
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
                            if (Attribute.FullName != null)
                            {
                                string name = attribute.FullName;
                                if (name.Length == 0) callName = "/";
                                else if (name[0] == '/') callName = IgnoreCase ? name.ToLower() : name;
                                else
                                {
                                    callName = "/" + name;
                                    if (IgnoreCase) callName = callName.toLower();
                                }
                            }
                            else
                            {
                                string name = attribute.MethodName ?? (MemberIndex == null ? Method.MethodName : MemberIndex.Member.Name);
                                callName = "/" + (name.Length != 0 ? TypeCallName.replace('.', '/') + "/" + name : TypeCallName.replace('.', '/'));
                                if (IgnoreCase) callName = callName.toLower();
                            }
                        }
                        return callName;
                    }
                }
                /// <summary>
                /// 最大接收数据字节数
                /// </summary>
                public int MaxPostDataSize
                {
                    get
                    {
                        return (Attribute == webCall.Null ? TypeAttribute : attribute).MaxPostDataSize << 20;
                    }
                }
                /// <summary>
                /// 内存流最大字节数
                /// </summary>
                public int MaxMemoryStreamSize
                {
                    get
                    {
                        return (Attribute == webCall.Null ? TypeAttribute : attribute).MaxMemoryStreamSize << 10;
                    }
                }
                /// <summary>
                /// 是否存在输入参数
                /// </summary>
                public bool IsParameter
                {
                    get
                    {
                        return MethodParameters.Length != 0;
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
                /// 方法参数
                /// </summary>
                private parameterInfo[] methodInputParameters;
                /// <summary>
                /// 方法参数
                /// </summary>
                internal parameterInfo[] MethodInputParameters
                {
                    get
                    {
                        if (methodInputParameters == null)
                        {
                            methodInputParameters = MethodParameters.getFindArray(value => !value.IsOut);
                        }
                        return methodInputParameters;
                    }
                }
                /// <summary>
                /// 是否存在输入参数
                /// </summary>
                public bool IsInputParameter
                {
                    get
                    {
                        return MethodInputParameters.Length != 0;
                    }
                }
                /// <summary>
                /// 异步回调是否检测成功状态
                /// </summary>
                protected override bool isAsynchronousFunc { get { return false; } }
                /// <summary>
                /// 是否AJAX加载器
                /// </summary>
                public bool IsAjaxLoad;
            }
            /// <summary>
            /// WEB调用函数
            /// </summary>
            private list<methodIndex> methods = new list<methodIndex>();
            /// <summary>
            /// AJAX函数
            /// </summary>
            public methodIndex[] Methods;
            ///// <summary>
            ///// Session类型
            ///// </summary>
            //public memberType SessionType;
            ///// <summary>
            ///// 安装入口
            ///// </summary>
            ///// <param name="parameter">安装参数</param>
            ///// <returns>是否安装成功</returns>
            //public override bool Run(auto.parameter parameter)
            //{
            //    webConfig webConfig = parameter.WebConfig;
            //    if (webConfig == null) return true;
            //    SessionType = webConfig.SessionType;
            //    return base.Run(parameter);
            //}
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

                bool isAjaxLoadType = false;
                Type baseType = type.Type.BaseType;
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(fastCSharp.code.cSharp.ajax.loader<>)) isAjaxLoadType = true;

                int methodIndex = methods.Count;
                methods.Add(code.methodInfo.GetMethods<fastCSharp.code.cSharp.webCall>(type, code.memberFilters.PublicInstance, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute)
                    .getArray(value => new methodIndex
                    {
                        Method = value,
                        MethodIndex = methodIndex++,
                        WebCallMethodType = type,
                        TypeAttribute = Attribute,
                        TypeCallName = typeCallName,
                        IsAjaxLoad = isAjaxLoadType && value.MemberName == "Load" && value.ReturnType.Type == typeof(void) && value.Parameters.Length == 0,
                        IgnoreCase = AutoParameter.WebConfig.IgnoreCase
                    }));
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected override void onCreated()
            {
                if (methods.Count != 0)
                {
                    Methods = methods.ToArray();
                    _code_.Empty();
                    create(false);
                    fastCSharp.code.coder.Add("namespace " + AutoParameter.DefaultNamespace + @"
{
" + _code_.ToString() + @"
}");
                }
            }
        }
    }
}
