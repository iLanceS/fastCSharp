using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using System.Net;

namespace fastCSharp.code
{
    /// <summary>
    /// TCP服务调用配置,定义类必须实现fastCSharp.code.cSharp.tcpServer.ITcpServer接口
    /// </summary>
    internal partial class tcpServer
    {
        /// <summary>
        /// TCP服务调用代码生成
        /// </summary
        [auto(Name = "TCP服务", DependType = typeof(coder.cSharper), IsAuto = true)]
        internal sealed partial class cSharp : tcpBase.cSharp<fastCSharp.code.cSharp.tcpServer>, IAuto
        {
            /// <summary>
            /// 默认TCP调用代码生成
            /// </summary>
            public static readonly cSharp Default = new cSharp();
            /// <summary>
            /// 是否生成服务器端代码
            /// </summary>
            public bool IsServerCode;
            /// <summary>
            /// 是否生成客户端代码
            /// </summary>
            public bool IsClientCode;
            /// <summary>
            /// 是否生成TCP服务命令序号记忆数据
            /// </summary>
            public bool IsRemember;
            /// <summary>
            /// 服务类名称
            /// </summary>
            public string ServiceNameOnly
            {
                get
                {
                    return Attribute.ServiceName.length() == 0 ? type.TypeOnlyName : Attribute.ServiceName;
                }
            }
            /// <summary>
            /// 负载均衡服务名称
            /// </summary>
            public string LoadBalancingServiceName
            {
                get { return ServiceNameOnly + "LoadBalancing"; }
            }
            /// <summary>
            /// 客户端附加接口类型
            /// </summary>
            public string ClientInterfaceType
            {
                get { return Attribute.ClientInterfaceType.fullName(); }
            }
            /// <summary>
            /// 服务类名称
            /// </summary>
            public override string ServiceName
            {
                get
                {
                    if (Attribute.ServiceName == null) return TypeName;
                    int index = TypeName.IndexOf('<');
                    if (index == -1) return ServiceNameOnly;
                    return ServiceNameOnly + TypeName.Substring(index);
                }
            }
            /// <summary>
            /// TCP服务器端配置
            /// </summary>
            public fastCSharp.code.cSharp.tcpServer ServiceAttribute
            {
                get { return Attribute; }
            }
            /// <summary>
            /// 调用参数位置
            /// </summary>
            public string ParameterPart
            {
                get { return Attribute.IsSegmentation ? clientPart : serverPart; }
            }
            /// <summary>
            /// 是否存在fastCSharp.code.cSharp.tcpServer.ICommandServer.SetCommandServer接口函数
            /// </summary>
            public bool IsSetCommandServer
            {
                get
                {
                    return typeof(fastCSharp.code.cSharp.tcpServer.ICommandServer).IsAssignableFrom(type.Type)
                        || type.Type.GetMethod("SetCommandServer", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(fastCSharp.net.tcp.commandServer) }, null) != null;
                }
            }
            /// <summary>
            /// TCP服务调用配置JSON
            /// </summary>
            public string AttributeJson
            {
                get
                {
                    return fastCSharp.emit.jsonSerializer.ToJson(Attribute).Replace(@"""", @"""""");
                }
            }
            /// <summary>
            /// 是否默认时间验证服务
            /// </summary>
            public bool IsTimeVerify
            {
                get { return typeof(fastCSharp.net.tcp.timeVerifyServer).IsAssignableFrom(type); }
            }
            /// <summary>
            /// 安装下一个类型
            /// </summary>
            protected unsafe override void nextCreate()
            {
                Attribute = fastCSharp.config.pub.LoadConfig(Attribute, Attribute.ServiceName);
                subArray<methodIndex> methodArray = code.methodInfo.GetMethods<fastCSharp.code.cSharp.tcpMethod>(type, Attribute.MemberFilter, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute)
                    .getArray(value => new methodIndex
                    {
                        Method = value,
                        MethodType = type,
                        ServiceAttribute = Attribute
                    }).toSubArray();
                foreach (memberIndex member in code.memberIndexGroup.Get<fastCSharp.code.cSharp.tcpMethod>(type, Attribute.MemberFilter, false, Attribute.IsAttribute, Attribute.IsBaseTypeAttribute, Attribute.IsInheritAttribute))
                {
                    if (member.IsField)
                    {
                        FieldInfo field = (FieldInfo)member.Member;
                        methodIndex getMethod = new methodIndex
                        {
                            Method = new methodInfo(field, true),
                            MemberIndex = member,
                            MethodType = type,
                            ServiceAttribute = Attribute
                        };
                        if (!getMethod.Attribute.IsOnlyGetMember)
                        {
                            getMethod.SetMethod = new methodIndex { Method = new methodInfo(field, false), MemberIndex = member, MethodType = type, ServiceAttribute = Attribute };
                        }
                        methodArray.Add(getMethod);
                        if (getMethod.SetMethod != null) methodArray.Add(getMethod.SetMethod);
                    }
                    else if (member.CanGet)
                    {
                        PropertyInfo property = (PropertyInfo)member.Member;
                        methodIndex getMethod = new methodIndex
                        {
                            Method = new methodInfo(property, true),
                            MemberIndex = member,
                            MethodType = type,
                            ServiceAttribute = Attribute
                        };
                        if (member.CanSet && !getMethod.Attribute.IsOnlyGetMember)
                        {
                            getMethod.SetMethod = new methodIndex { Method = new methodInfo(property, false), MemberIndex = member, MethodType = type, ServiceAttribute = Attribute };
                        }
                        methodArray.Add(getMethod);
                        if (getMethod.SetMethod != null) methodArray.Add(getMethod.SetMethod);
                    }
                }
                MethodIndexs = methodArray.ToArray();
                if (Attribute.IsIdentityCommand)
                {
                    MethodIndexs = methodIndex.CheckIdentity(MethodIndexs, Attribute.IsRememberIdentityCommand ? getRememberIdentityName() : nullRememberIdentityName, method => method.Method.MethodKeyFullName);
                    if (MethodIndexs == null) return;
                    MaxCommandLength = sizeof(int) * 5 + sizeof(fastCSharp.net.tcp.commandServer.streamIdentity);
                }
                else
                {
                    MaxCommandLength = MethodIndexs.maxKey(value => (value.Method.MethodKeyFullName.Length + 3) & (int.MaxValue - 3), sizeof(int)) + sizeof(int) * 4 + sizeof(fastCSharp.net.tcp.commandServer.streamIdentity);
                }
                int index = 0;
                foreach (methodIndex method in MethodIndexs) method.MethodIndex = method.ParameterIndex = index++;
                methodIndex[] methodIndexs = MethodIndexs.getFindArray(value => !value.IsNullMethod);
                if (ServiceAttribute.IsHttpClient && !methodIndex.CheckHttpMethodName(methodIndexs)) return;
                IsVerifyMethod = methodIndexs.any(value => value.IsVerifyMethod);
                subArray<int> groupIds = methodIndexs.distinct(value => value.Attribute.GroupId);
                groupIds.Remove(0);
                MethodGroups = groupIds.GetArray(value => new methodGroup { GroupId = value });
                IsRemember = false;
                if (ServiceAttribute.IsSegmentation)
                {
                    IsClientCode = false;
                    create(IsServerCode = true);
                    definition.cSharp definition = new definition.cSharp(type, IsClientCode = true, false, type.Type.Namespace + ".tcpClient");
                    _code_.Empty();
                    _code_.Add(definition.Start);
                    create(IsServerCode = false);
                    _code_.Add(definition.End);
                    string fileName = AutoParameter.ProjectPath + "{" + AutoParameter.DefaultNamespace + "}.tcpServer." + Attribute.ServiceName + ".client.cs";
                    string clientCode = fastCSharp.code.coder.WarningCode + _code_.ToString() + fastCSharp.code.coder.FileEndCode;
                    if (fastCSharp.code.coder.WriteFile(fileName, clientCode))
                    {
                        if (ServiceAttribute.ClientSegmentationCopyPath != null)
                        {
                            string copyFileName = ServiceAttribute.ClientSegmentationCopyPath + "{" + AutoParameter.DefaultNamespace + "}.tcpServer." + Attribute.ServiceName + ".client.cs";
                            if (!fastCSharp.code.coder.WriteFile(copyFileName, clientCode)) fastCSharp.code.error.Add(copyFileName + " 写入失败");
                        }
                        fastCSharp.code.error.Message(fileName + " 被修改");
                    }
                }
                else create(IsServerCode = IsClientCode = true);
                if (Attribute.IsIdentityCommand && Attribute.IsRememberIdentityCommand)
                {
                    IsRemember = true;
                    _code_.Empty();
                    create(IsServerCode = IsClientCode = false);
                    coder.AddRemember(@"
namespace " + AutoParameter.DefaultNamespace + @".tcpRemember
{
" + _partCodes_["REMEMBER"] + @"
}");
                }
            }
            /// <summary>
            /// 安装完成处理
            /// </summary>
            protected override void onCreated()
            {
            }
        }
    }
}
