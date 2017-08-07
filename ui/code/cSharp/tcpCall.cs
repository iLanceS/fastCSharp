using System;
using System.Collections.Generic;
using fastCSharp.net;
using fastCSharp.reflection;
using System.Reflection;
using System.Threading;
using fastCSharp.threading;

namespace fastCSharp.code
{
    /// <summary>
    /// TCP调用配置
    /// </summary>
    internal partial class tcpCall
    {
        /// <summary>
        /// TCP调用代码生成
        /// </summary
        [auto(Name = "TCP调用", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : tcpBase.cSharp<fastCSharp.code.cSharp.tcpCall>, IAuto
        {
            /// <summary>
            /// 默认TCP调用代码生成
            /// </summary>
            public static readonly cSharp Default = new cSharp();
            /// <summary>
            /// TCP调用服务
            /// </summary>
            private sealed class server
            {
                /// <summary>
                /// TCP调用类型
                /// </summary>
                public class type
                {
                    /// <summary>
                    /// TCP调用类型
                    /// </summary>
                    public memberType Type;
                    /// <summary>
                    /// TCP调用配置
                    /// </summary>
                    public fastCSharp.code.cSharp.tcpCall Attribute;
                    /// <summary>
                    /// 方法集合
                    /// </summary>
                    public list<methodIndex> Methods = new list<methodIndex>();
                }
                /// <summary>
                /// TCP调用服务配置
                /// </summary>
                public fastCSharp.code.cSharp.tcpServer TcpServer = new fastCSharp.code.cSharp.tcpServer();
                /// <summary>
                /// 类型集合
                /// </summary>
                public subArray<type> Types;
                /// <summary>
                /// 配置类型
                /// </summary>
                public memberType AttributeType;
                /// <summary>
                /// 是否存在方法
                /// </summary>
                public bool IsMethod;
                /// <summary>
                /// 是否默认时间验证服务
                /// </summary>
                public bool IsTimeVerify
                {
                    get { return typeof(fastCSharp.net.tcp.timeVerifyServer).IsAssignableFrom(AttributeType); }
                }
            }
            /// <summary>
            /// 是否搜索父类属性
            /// </summary>
            public override bool IsBaseType
            {
                get { return true; }
            }
            /// <summary>
            /// 自定义属性是否可继承
            /// </summary>
            public override bool IsInheritAttribute
            {
                get { return true; }
            }
            /// <summary>
            /// 类型泛型参数类型名称
            /// </summary>
            public string TypeGenericParameterName
            {
                get { return typeGenericParameterName; }
            }
            /// <summary>
            /// 方法索引集合
            /// </summary>
            private list<methodIndex> methodIndexs = new list<methodIndex>();
            /// <summary>
            /// TCP服务器端配置
            /// </summary>
            public fastCSharp.code.cSharp.tcpServer ServiceAttribute;
            /// <summary>
            /// TCP验证类型
            /// </summary>
            public override string TcpVerifyType
            {
                get { return ServiceAttribute.VerifyType.fullName(); }
            }
            /// <summary>
            /// TCP客户端验证方法类型
            /// </summary>
            public override string TcpVerifyMethodType
            {
                get { return ServiceAttribute.VerifyMethodType.fullName(); }
            }
            /// <summary>
            /// 客户端验证方法类型
            /// </summary>
            public string TcpTimeVerifyMethodType
            {
                get { return type.FullName; }
            }
            /// <summary>
            /// 调用参数位置
            /// </summary>
            public string ParameterPart
            {
                get { return ServiceAttribute.IsSegmentation ? clientPart : serverPart; }
            }
            /// <summary>
            /// 配置类型
            /// </summary>
            public string TcpServerAttributeType;
            /// <summary>
            /// TCP调用服务集合
            /// </summary>
            private Dictionary<hashString, server> servers = dictionary.CreateHashString<server>();
            /// <summary>
            /// TCP调用服务类型集合
            /// </summary>
            private Dictionary<hashString, server.type> serverTypes = dictionary.CreateHashString<server.type>();
            /// <summary>
            /// 参数索引
            /// </summary>
            private int parameterIndex;
            /// <summary>
            /// 服务类名称
            /// </summary>
            public override string ServiceName { get { return ServiceAttribute.ServiceName; } }

            /// <summary>
            /// 服务类名称(临时变量)
            /// </summary>
            private string defaultServiceName;
            /// <summary>
            /// TCP调用服务(临时变量)
            /// </summary>
            private server defaultServer;
            /// <summary>
            /// TCP调用类型(临时变量)
            /// </summary>
            private server.type defaultType;
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected override void nextCreate()
            {
                if (Attribute.IsAbstract || type.Type.IsSealed || !type.Type.IsAbstract)
                {
                    defaultServiceName = Attribute.ServiceName;
                    defaultServer = null;
                    defaultType = null;
                    if (defaultServiceName != null)
                    {
                        hashString nameKey = defaultServiceName;
                        if (!servers.TryGetValue(nameKey, out defaultServer)) servers.Add(nameKey, defaultServer = new server());
                        defaultServer.TcpServer.Service = defaultServiceName;
                        defaultServer.Types.Add(defaultType = new server.type { Type = type, Attribute = Attribute });
                        if (Attribute.IsServer)
                        {
                            defaultServer.AttributeType = type;
                            defaultServer.TcpServer.CopyFrom(Attribute);
                        }
                    }
                    foreach (methodInfo method in code.methodInfo.GetMethods<fastCSharp.code.cSharp.tcpMethod>(type, Attribute.MemberFilter, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute))
                    {
                        next(new methodIndex
                        {
                            Method = method,
                            MethodType = type,
                            IsTypeGenericParameterName = type.Type.IsGenericType
                        });
                    }
                    if (!type.Type.IsGenericType)
                    {
                        foreach (memberIndex member in code.memberIndexGroup.GetStatic<fastCSharp.code.cSharp.tcpMethod>(type, Attribute.MemberFilter, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute))
                        {
                            if (member.IsField)
                            {
                                FieldInfo field = (FieldInfo)member.Member;
                                methodIndex getMethod = new methodIndex
                                {
                                    Method = new methodInfo(field, true),
                                    MemberIndex = member,
                                    MethodType = type
                                };
                                if (!getMethod.Attribute.IsOnlyGetMember)
                                {
                                    getMethod.SetMethod = new methodIndex { Method = new methodInfo(field, false), MemberIndex = member, MethodType = type };
                                }
                                next(getMethod);
                                if (getMethod.SetMethod != null) next(getMethod.SetMethod);
                            }
                            else if (member.CanGet)
                            {
                                PropertyInfo property = (PropertyInfo)member.Member;
                                methodIndex getMethod = new methodIndex
                                {
                                    Method = new methodInfo(property, true),
                                    MemberIndex = member,
                                    MethodType = type
                                };
                                if (member.CanSet && !getMethod.Attribute.IsOnlyGetMember)
                                {
                                    getMethod.SetMethod = new methodIndex { Method = new methodInfo(property, false), MemberIndex = member, MethodType = type };
                                }
                                next(getMethod);
                                if (getMethod.SetMethod != null) next(getMethod.SetMethod);
                            }
                        }
                    }
                    serverTypes.Clear();
                }
            }
            /// <summary>
            /// 下一个函数处理
            /// </summary>
            /// <param name="methodIndex"></param>
            private void next(methodIndex methodIndex)
            {
                fastCSharp.code.cSharp.tcpMethod attribute = methodIndex.Attribute;
                server server = defaultServer;
                server.type serverType = defaultType;
                string serviceName = attribute.ServiceName;
                if (serviceName == null) serviceName = Attribute.ServiceName;
                if (serviceName != defaultServiceName)
                {
                    if (serviceName == null) serverType = null;
                    else
                    {
                        hashString nameKey = serviceName;
                        if (!servers.TryGetValue(nameKey, out server))
                        {
                            servers.Add(nameKey, server = new server());
                            server.TcpServer.Service = serviceName;
                        }
                        if (!serverTypes.TryGetValue(nameKey, out serverType))
                        {
                            server.Types.Add(serverType = new server.type { Type = type });
                            serverTypes.Add(nameKey, serverType);
                        }
                    }
                }
                if (serverType != null)
                {
                    server.IsMethod = true;
                    methodIndex.ServiceAttribute = server.TcpServer;
                    //methodIndex.MethodIndex = server.MethodIndex++;
                    methodIndex.ParameterIndex = parameterIndex++;
                    serverType.Methods.Add(methodIndex);
                }
            }
            /// <summary>
            /// 是否所有类型
            /// </summary>
            public bool IsAllType;
            /// <summary>
            /// 是否默认时间验证服务
            /// </summary>
            public bool IsTimeVerify;
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected unsafe override void onCreated()
            {
                stringBuilder clientCallCode = new stringBuilder();
                list<methodIndex> methods = new list<methodIndex>();
                methodIndex[] methodIndexs;
                foreach (server server in servers.Values)
                {
                    if (server.IsMethod)
                    {
                        IsAllType = false;
                        TcpServerAttributeType = server.AttributeType == null || server.AttributeType.Type == null ? null : server.AttributeType.FullName;
                        ServiceAttribute = fastCSharp.config.pub.LoadConfig(server.TcpServer, server.TcpServer.ServiceName);
                        foreach (server.type serverType in server.Types) methods.Add(serverType.Methods);
                        methodIndexs = methods.ToArray();
                        methods.Empty();
                        if (ServiceAttribute.IsIdentityCommand)
                        {
                            methodIndexs = methodIndex.CheckIdentity(methodIndexs, ServiceAttribute.IsRememberIdentityCommand ? getRememberIdentityName() : nullRememberIdentityName, method => method.Method.MethodKeyFullName);
                            if (methodIndexs == null) return;
                        }
                        int index = 0;
                        foreach (methodIndex method in methodIndexs) method.MethodIndex = index++;
                        foreach (server.type serverType in server.Types)
                        {
                            if (serverType.Methods.Count != 0)
                            {
                                type = serverType.Type;
                                IsTimeVerify = type == server.AttributeType && server.IsTimeVerify;
                                Attribute = serverType.Attribute ?? new code.cSharp.tcpCall();
                                MethodIndexs = serverType.Methods.ToArray();
                                definition.cSharp definition = new definition.cSharp(type, true, false);
                                _code_.Empty();
                                create(false);
                                fastCSharp.code.coder.Add(definition.Start + _partCodes_["SERVERCALL"] + definition.End);
                                if (ServiceAttribute.IsSegmentation)
                                {
                                    clientCallCode.Add(definition.Start + _partCodes_["CLIENTCALL"] + definition.End);
                                }
                                else fastCSharp.code.coder.Add(definition.Start + _partCodes_["CLIENTCALL"] + definition.End);
                            }
                        }
                        IsAllType = true;
                        MethodIndexs = methodIndexs;
                        methodIndexs = methodIndexs.getFindArray(value => !value.IsNullMethod);
                        if (ServiceAttribute.IsHttpClient && !methodIndex.CheckHttpMethodName(methodIndexs)) return;
                        IsVerifyMethod = methodIndexs.any(value => value.IsVerifyMethod);
                        subArray<int> groupIds = methodIndexs.distinct(value => value.Attribute.GroupId);
                        groupIds.Remove(0);
                        MethodGroups = groupIds.GetArray(value => new methodGroup { GroupId = value });
                        MaxCommandLength = (ServiceAttribute.IsIdentityCommand ? sizeof(int) : methodIndexs.maxKey(value => (value.Method.MethodKeyFullName.Length + 3) & (int.MaxValue - 3), sizeof(int))) + sizeof(int) * 4 + sizeof(fastCSharp.net.tcp.commandServer.streamIdentity);
                        _code_.Empty();
                        create(false);
                        fastCSharp.code.coder.Add(@"
namespace " + AutoParameter.DefaultNamespace + "." + serverPart + @"
{
" + _partCodes_["SERVER"] + @"
}");
                        string clientCode = @"
namespace " + AutoParameter.DefaultNamespace + "." + clientPart + @"
{
" + _partCodes_["CLIENT"] + @"
}";
                        if (ServiceAttribute.IsIdentityCommand && ServiceAttribute.IsRememberIdentityCommand)
                        {
                            coder.AddRemember(@"
namespace " + AutoParameter.DefaultNamespace + @".tcpRemember
{
" + _partCodes_["REMEMBER"] + @"
}");
                        }
                        if (ServiceAttribute.IsSegmentation)
                        {
                            clientCallCode.Add(clientCode);
                            string fileName = AutoParameter.ProjectPath + "{" + AutoParameter.DefaultNamespace + "}.tcpCall." + ServiceAttribute.ServiceName + ".client.cs";
                            clientCode = fastCSharp.code.coder.WarningCode + clientCallCode.ToString() + fastCSharp.code.coder.FileEndCode;
                            if (fastCSharp.code.coder.WriteFile(fileName, clientCode))
                            {
                                if (ServiceAttribute.ClientSegmentationCopyPath != null)
                                {
                                    string copyFileName = ServiceAttribute.ClientSegmentationCopyPath + "{" + AutoParameter.DefaultNamespace + "}.tcpCall." + ServiceAttribute.ServiceName + ".client.cs";
                                    if (!fastCSharp.code.coder.WriteFile(copyFileName, clientCode)) fastCSharp.code.error.Add(copyFileName + " 写入失败");
                                }
                                fastCSharp.code.error.Message(fileName + " 被修改");
                            }
                            clientCallCode.Empty();
                        }
                        else fastCSharp.code.coder.Add(clientCode);
                    }
                }
            }
        }
    }
}
