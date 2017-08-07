using System;
using System.Reflection;
using System.Net;
using System.Threading;
using fastCSharp.reflection;
using fastCSharp.threading;
using fastCSharp.net.tcp;
using System.Text;
using System.IO;
using fastCSharp;
using fastCSharp.net.tcp.http;
using System.Collections.Generic;

namespace fastCSharp.code
{
    /// <summary>
    /// TCP调用配置基类
    /// </summary>
    internal abstract partial class tcpBase
    {
        /// <summary>
        /// 泛型返回值类型名称
        /// </summary>
        protected const string ReturnParameterTypeName = "_ReturnType_";
        /// <summary>
        /// TCP调用代码生成
        /// </summary>
        internal abstract class cSharp<attributeType> : cSharper<attributeType> where attributeType : fastCSharp.code.cSharp.tcpBase
        {
            /// <summary>
            /// 默认空属性
            /// </summary>
            private static readonly fastCSharp.emit.jsonParse parseJsonAttribute = new fastCSharp.emit.jsonParse { IsAllMember = true, IsBaseTypeAttribute = true };
            /// <summary>
            /// 方法泛型参数类型名称
            /// </summary>
            private const string genericParameterTypeName = "_GenericParameterTypes_";
            /// <summary>
            /// 类型泛型参数类型名称
            /// </summary>
            protected const string typeGenericParameterName = "_TypeGenericParameter_";
            /// <summary>
            /// 服务器端位置
            /// </summary>
            protected const string serverPart = "tcpServer";
            /// <summary>
            /// 客户端位置
            /// </summary>
            protected const string clientPart = "tcpClient";
            /// <summary>
            /// 负载均衡位置
            /// </summary>
            protected const string loadBalancingPart = "tcpLoadBalancing";
            /// <summary>
            /// 方法索引信息
            /// </summary>
            public sealed class methodIndex : asynchronousMethod
            {
                /// <summary>
                /// 是否空方法
                /// </summary>
                public bool IsNullMethod;
                /// <summary>
                /// 检测异步回调方法
                /// </summary>
                protected override void checkAsynchronousReturn()
                {
                    if (methodParameters == null)
                    {
                        base.checkAsynchronousReturn();
                        if (methodParameters.Length != 0)
                        {
                            foreach (parameterInfo parameterInfo in methodParameters)
                            {
                                if (parameterInfo.ParameterType.IsStream)
                                {
                                    if (parameterInfo.IsRefOrOut) error.Add(parameterInfo.ParameterName + " Stream不支持引用传递方式");
                                }
                                if (attribute.IsClientAsynchronousReturnInputParameter && ReturnInputParameterName == null && parameterInfo.ParameterType.Type == methodReturnType.Type)
                                {
                                    ReturnInputParameterName = parameterInfo.ParameterName;
                                }
                            }
                            if (!methodParameters[0].IsRefOrOut && methodParameters[0].ParameterType.Type == typeof(commandServer.socket))
                            {
                                clientParameterName = methodParameters[0].ParameterName;
                                methodParameters = parameterInfo.Get(methodParameters.getSub(1, methodParameters.Length - 1));
                            }
                        }
                    }
                }
                /// <summary>
                /// 返回值绑定输入参数名称
                /// </summary>
                public string ReturnInputParameterName;
                /// <summary>
                /// 客户端标识参数名称
                /// </summary>
                private string clientParameterName;
                /// <summary>
                /// 客户端标识参数名称
                /// </summary>
                public string ClientParameterName
                {
                    get
                    {
                        checkAsynchronousReturn();
                        return clientParameterName;
                    }
                }
                /// <summary>
                /// 获取该方法的类型
                /// </summary>
                public memberType MethodType;
                /// <summary>
                /// 属性或者字段设置值函数信息
                /// </summary>
                public methodIndex SetMethod;
                /// <summary>
                /// 服务方法索引
                /// </summary>
                public int MethodIndex;
                /// <summary>
                /// 参数索引
                /// </summary>
                public int ParameterIndex;
                /// <summary>
                /// 是否处理类型泛型参数类型名称
                /// </summary>
                public bool IsTypeGenericParameterName;
                /// <summary>
                /// TCP服务器端配置
                /// </summary>
                public fastCSharp.code.cSharp.tcpServer ServiceAttribute;
                /// <summary>
                /// TCP调用配置
                /// </summary>
                private fastCSharp.code.cSharp.tcpMethod attribute;
                /// <summary>
                /// TCP调用配置
                /// </summary>
                public fastCSharp.code.cSharp.tcpMethod Attribute
                {
                    get
                    {
                        if (attribute == null)
                        {
                            attribute = (MemberIndex ?? Method).GetSetupAttribute<fastCSharp.code.cSharp.tcpMethod>(false, true);
                            if (MemberIndex != null && !Method.IsGetMember) attribute = attribute.Clone();
                            //attribute = fastCSharp.ui.reflection.memberInfo.customAttribute<fastCSharp.code.cSharp.tcpMethod>(Method.Method, false, true);
                        }
                        if (ServiceAttribute != null && ServiceAttribute.IsJsonSerialize) attribute.IsJsonSerialize = true;
                        return attribute;
                    }
                }
                /// <summary>
                /// 方法索引名称
                /// </summary>
                public string MethodIndexName
                {
                    get
                    {
                        return "_M" + MethodIndex.toString();
                    }
                }
                ///// <summary>
                ///// 同步方法名称
                ///// </summary>
                //public string WaitMethodName
                //{
                //    get
                //    {
                //        return Method.MethodGenericName;
                //    }
                //}
                /// <summary>
                /// 参数索引名称
                /// </summary>
                public string ParameterIndexName
                {
                    get
                    {
                        return "_M" + ParameterIndex.toString();
                    }
                }
                /// <summary>
                /// 方法索引泛型名称
                /// </summary>
                public string MethodIndexGenericName
                {
                    get
                    {
                        return ParameterIndexName + Method.GenericParameterName;
                    }
                }
                /// <summary>
                /// 异步回调方返回值是否泛型
                /// </summary>
                public bool IsGenericParameterCallback
                {
                    get
                    {
                        return IsAsynchronousCallback && MethodReturnType.Type.IsGenericParameter;
                    }
                }
                /// <summary>
                /// 是否存在输入参数
                /// </summary>
                public bool IsInputParameter
                {
                    get
                    {
                        return MethodParameters.Length != 0 || Method.GenericParameters.Length != 0 || (IsAsynchronousCallback && MethodReturnType.Type.IsGenericParameter) || IsTypeGenericParameterName;
                    }
                }
                /// <summary>
                /// 输入参数最大数据长度
                /// </summary>
                public int InputParameterMaxLength
                {
                    get
                    {
                        return IsInputParameter || Attribute.InputParameterMaxLength != 0 ? (Attribute.InputParameterMaxLength == 0 ? int.MaxValue : Attribute.InputParameterMaxLength) : 0;
                    }
                }
                /// <summary>
                /// 是否存在输入参数最大数据长度
                /// </summary>
                public bool IsInputParameterMaxLength
                {
                    get { return !IsInputParameter || Attribute.InputParameterMaxLength != 0; }
                }
                /// <summary>
                /// 是否存在输出参数
                /// </summary>
                public bool IsOutputParameter
                {
                    get
                    {
                        return MethodReturnType.Type != typeof(void) || Method.OutputParameters.Length != 0;
                    }
                }
                /// <summary>
                /// 输入参数名称
                /// </summary>
                public string InputParameterTypeName
                {
                    get
                    {
                        return "_i" + MethodIndex.toString();
                    }
                }
                ///// <summary>
                ///// 输入参数是否引用类型
                ///// </summary>
                //public bool IsInputParameterClass
                //{
                //    get { return MethodParameters.Length > (IsGenericParameterCallback ? 3 : 4); }
                //}
                /// <summary>
                /// 输入参数类型名称class/struct
                /// </summary>
                public string InputParameterClassType
                {
                    get
                    {
                        return Attribute.IsInputParameterClass ? "sealed class" : "struct";
                    }
                }
                /// <summary>
                /// 泛型返回值参数类型名称
                /// </summary>
                public string ReturnTypeName
                {
                    get { return ReturnParameterTypeName; }
                }
                /// <summary>
                /// 输出参数名称
                /// </summary>
                public string OutputParameterTypeName
                {
                    get
                    {
                        return "_o" + MethodIndex.toString();
                    }
                }
                ///// <summary>
                ///// 输出参数是否引用类型
                ///// </summary>
                //public bool IsOutputParameterClass
                //{
                //    get { return Method.OutputParameters.Length > (MethodIsReturn ? 3 : 4); }
                //}
                /// <summary>
                /// 输出参数类型名称class/struct
                /// </summary>
                public string OutputParameterClassType
                {
                    get
                    {
                        return Attribute.IsOutputParameterClass ? "sealed class" : "struct";
                    }
                }
                /// <summary>
                /// 输出参数名称
                /// </summary>
                public string OutputParameterGenericTypeName
                {
                    get
                    {
                        return OutputParameterTypeName;
                    }
                }
                /// <summary>
                /// JSON序列化调用
                /// </summary>
                public string JsonCall
                {
                    get { return Attribute.IsJsonSerialize ? "Json" : null; }
                }
                /// <summary>
                /// 泛型函数信息名称
                /// </summary>
                public string GenericMethodInfoName
                {
                    get
                    {
                        return "_g" + ParameterIndex.toString();
                    }
                }
                /// <summary>
                /// TCP调用命令名称
                /// </summary>
                public string MethodIdentityCommand
                {
                    get
                    {
                        return "_c" + ParameterIndex.toString();
                    }
                }
                /// <summary>
                /// TCP调用命令名称
                /// </summary>
                public string MethodDataCommand
                {
                    get
                    {
                        return MethodIdentityCommand;
                    }
                }
                /// <summary>
                /// 客户端调用是否支持异步
                /// </summary>
                public bool IsClientAsynchronous
                {
                    get
                    {
                        return (Attribute.IsClientAsynchronous && IsOutputParameter) || IsKeepCallback != 0;
                        //return IsOutputParameter || IsKeepCallback;
                    }
                }
                /// <summary>
                /// 客户端调用是否支持同步
                /// </summary>
                public bool IsClientSynchronous
                {
                    get
                    {
                        return Attribute.IsClientSynchronous && IsKeepCallback == 0;
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
                /// 是否泛型类型 或者 函数是否拥有输入参数
                /// </summary>
                public bool IsGenericTypeOrParameter
                {
                    get
                    {
                        return MethodType.Type.IsGenericType && Method.Parameters.Length != 0;
                    }
                }
                /// <summary>
                /// 引用参数是否需要反射泛型调用
                /// </summary>
                public bool IsInvokeGenericMethod
                {
                    get
                    {
                        return MethodType.Type.IsGenericType || (MemberIndex == null && Method.Method.IsGenericMethod);
                    }
                }
                /// <summary>
                /// 是否验证方法
                /// </summary>
                public bool IsVerifyMethod
                {
                    get
                    {
                        if (Attribute.IsVerifyMethod)
                        {
                            if (MethodReturnType.Type == typeof(bool))
                            {
                                if (MethodParameters.Length != 0) return true;
                                error.Message("方法 " + MemberFullName + " 没有输入参数,不符合验证方法要求");
                            }
                            else error.Message("方法 " + MemberFullName + " 的返回值类型为 " + MethodReturnType.Type.fullName() + " ,不符合验证方法要求");
                        }
                        return false;
                    }
                }
                /// <summary>
                /// 服务器端模拟异步调用是否使用任务池
                /// </summary>
                public bool IsServerAsynchronousTask
                {
                    get
                    {
                        if (ServiceAttribute.IsServerAsynchronousReceive) return Attribute.IsServerSynchronousTask;
                        return Attribute.IsServerSynchronousTask && !IsVerifyMethod;
                    }
                }
                /// <summary>
                /// 流式套接字调用索引名称
                /// </summary>
                public string MethodStreamName
                {
                    get
                    {
                        return "_s" + MethodIndex.toString();// +Method.GenericParameterName;
                    }
                }
                /// <summary>
                /// 合并命令套接字调用索引名称
                /// </summary>
                public string MethodMergeName
                {
                    get
                    {
                        return "_m" + MethodIndex.toString();// +Method.GenericParameterName;
                    }
                }
                /// <summary>
                /// 负载均衡回调名称
                /// </summary>
                public string LoadBalancingCallbackName
                {
                    get
                    {
                        return "_l" + MethodIndex.toString();
                    }
                }
                /// <summary>
                /// 空方法索引信息
                /// </summary>
                private static readonly methodIndex nullMethod = new methodIndex { IsNullMethod = true };
                /// <summary>
                /// 检测方法序号
                /// </summary>
                /// <param name="methodIndexs">方法集合</param>
                /// <param name="rememberIdentityCommand">命令序号记忆数据</param>
                /// <param name="getMethodKeyName">获取命令名称的委托</param>
                /// <param name="nullMethod">空方法索引信息</param>
                /// <returns>方法集合,失败返回null</returns>
                public static methodIndex[] CheckIdentity(methodIndex[] methodIndexs, staticDictionary<string, int> rememberIdentityCommand, Func<methodIndex, string> getMethodKeyName)
                {
                    if (!rememberIdentityCommand.IsEmpty())
                    {
                        foreach (methodIndex method in methodIndexs)
                        {
                            if (method.Attribute.CommandIentity == int.MaxValue)
                            {
                                int identity = rememberIdentityCommand.Get(getMethodKeyName(method), int.MaxValue);
                                if (identity != int.MaxValue) method.Attribute.CommandIentity = identity;
                            }
                        }
                    }
                    methodIndex[] setMethodIndexs = methodIndexs.getFindArray(value => value.Attribute.CommandIentity != int.MaxValue);
                    int count = 0;
                    foreach (methodIndex method in setMethodIndexs)
                    {
                        int identity = method.Attribute.CommandIentity;
                        if (identity != int.MaxValue && identity > count) count = identity;
                    }
                    if (++count < methodIndexs.Length) count = methodIndexs.Length;
                    methodIndex[] sortMethodIndexs = new methodIndex[count];
                    foreach (methodIndex method in setMethodIndexs)
                    {
                        int identity = method.Attribute.CommandIentity;
                        if (sortMethodIndexs[identity] == null) sortMethodIndexs[identity] = method;
                        else
                        {
                            error.Add(method.MethodType.FullName + " 命令序号重复 " + method.MemberFullName + "[" + identity.toString() + "]" + sortMethodIndexs[identity].MemberFullName);
                            return null;
                        }
                    }
                    count = 0;
                    foreach (methodIndex method in methodIndexs.getFind(value => value.Attribute.CommandIentity == int.MaxValue))
                    {
                        while (sortMethodIndexs[count] != null) ++count;
                        (sortMethodIndexs[count] = method).Attribute.CommandIentity = count;
                        ++count;
                    }
                    while (count != sortMethodIndexs.Length)
                    {
                        if (sortMethodIndexs[count] == null) sortMethodIndexs[count] = nullMethod;
                        ++count;
                    }
                    return sortMethodIndexs;
                    //methodIndexs = methodIndexs.getSort(value => value.Attribute.CommandIentity);
                    //for (int index = 0; index != methodIndexs.Length; ++index)
                    //{
                    //    methodIndex method = methodIndexs[index];
                    //    int identity = method.Attribute.CommandIentity;
                    //    while (identity != index)
                    //    {
                    //        if (identity == int.MaxValue)
                    //        {
                    //            methodIndexs[index] = method;
                    //            method.CopyTypeAttribute.CommandIentity = identity = index;
                    //        }
                    //        else if (identity < methodIndexs.Length)
                    //        {
                    //            methodIndex moveMethod = methodIndexs[identity];
                    //            int moveIdentity = moveMethod.Attribute.CommandIentity;
                    //            if (moveIdentity == identity)
                    //            {
                    //                error.Add(method.MethodType.FullName + " 命令序号重复 " + method.Method.Method.Name + "[" + identity.toString() + "]" + moveMethod.Method.Method.Name);
                    //                return null;
                    //            }
                    //            methodIndexs[identity] = method;
                    //            identity = moveIdentity;
                    //            method = moveMethod;
                    //        }
                    //        else
                    //        {
                    //            error.Add(method.MethodType.FullName + " 命令序号错误 " + method.Method.Method.Name + "[" + identity.toString() + "] >= " + methodIndexs.Length.toString());
                    //            return null;
                    //        }
                    //    }
                    //}
                    //return methodIndexs;
                }
                /// <summary>
                /// HTTP调用名称冲突检测
                /// </summary>
                /// <param name="methodIndexs">方法集合</param>
                /// <returns>是否成功</returns>
                public static bool CheckHttpMethodName(methodIndex[] methodIndexs)
                {
                    Dictionary<string, list<methodIndex>> groups = methodIndexs.getFindArray(value => !value.IsNullMethod).group(value => value.HttpMethodName);
                    if (groups.Count == methodIndexs.Length) return true;
                    foreach (KeyValuePair<string, list<methodIndex>> group in groups)
                    {
                        if (group.Value.Count != 1) error.Add("HTTP调用命令冲突 " + group.Key + "[" + group.Value.Count.toString() + "]");
                    }
                    return false;
                }
                /// <summary>
                /// 保持异步回调类型名称
                /// </summary>
                private static readonly string keepCallbackType = typeof(commandClient.streamCommandSocket.keepCallback).fullName();
                /// <summary>
                /// 是否保持异步回调
                /// </summary>
                public int IsKeepCallback
                {
                    get
                    {
                        return Attribute.IsKeepCallback && !IsVerifyMethod ? 1 : 0;
                    }
                }
                /// <summary>
                /// 是否仅发送数据
                /// </summary>
                public int IsClientSendOnly
                {
                    get
                    {
                        if (Attribute.IsClientSendOnly && !IsClientAsynchronous)
                        {
                            checkAsynchronousReturn();
                            if (MethodReturnType.Type == typeof(void)) return 1;
                        }
                        return 0;
                    }
                }
                /// <summary>
                /// 是否定义服务器端调用
                /// </summary>
                public bool IsMethodServerCall
                {
                    get
                    {
                        return !IsAsynchronousCallback && (IsServerAsynchronousTask || MemberIndex != null || IsClientSendOnly == 0 || Method.Method.IsGenericMethod);
                    }
                }
                /// <summary>
                /// 保持异步回调类型名称
                /// </summary>
                public string KeepCallbackType
                {
                    get
                    {
                        return IsKeepCallback == 0 ? "void" : keepCallbackType;
                    }
                }
                /// <summary>
                /// 保持异步回调
                /// </summary>
                public string KeepCallback
                {
                    get { return IsKeepCallback == 0 ? null : "Keep"; }
                }
                /// <summary>
                /// HTTP调用名称
                /// </summary>
                public string HttpMethodName
                {
                    get
                    {
                        if (Attribute.HttpName != null) return Attribute.HttpName;
                        if (typeof(attributeType) == typeof(fastCSharp.code.cSharp.tcpServer)) return MemberIndex == null ? Method.Method.Name : MemberIndex.Member.Name;
                        return MethodType.Type.Name + "." + (MemberIndex == null ? Method.Method.Name : MemberIndex.Member.Name);
                    }
                }
                /// <summary>
                /// 分组计数名称
                /// </summary>
                public string GroupCountName
                {
                    get { return new methodGroup { GroupId = Attribute.GroupId }.GroupCountName; }
                }
                /// <summary>
                /// 分组忽略名称
                /// </summary>
                public string GroupIgnoreName
                {
                    get { return new methodGroup { GroupId = Attribute.GroupId }.GroupIgnoreName; }
                }
            }
            /// <summary>
            /// 方法分组
            /// </summary>
            public struct methodGroup
            {
                /// <summary>
                /// 分组标识
                /// </summary>
                public int GroupId;
                /// <summary>
                /// 分组计数名称
                /// </summary>
                public string GroupCountName
                {
                    get { return "_gc" + GroupId.toString(); }
                }
                /// <summary>
                /// 分组忽略名称
                /// </summary>
                public string GroupIgnoreName
                {
                    get { return "_gi" + GroupId.toString(); }
                }
            }
            /// <summary>
            /// 方法索引集合
            /// </summary>
            public methodIndex[] MethodIndexs;
            /// <summary>
            /// 方法分组集合
            /// </summary>
            public methodGroup[] MethodGroups;
            /// <summary>
            /// 方法泛型参数类型名称
            /// </summary>
            public string GenericParameterTypeName
            {
                get { return genericParameterTypeName; }
            }
            /// <summary>
            /// 泛型类型服务器端调用类型名称
            /// </summary>
            public string GenericTypeServerName
            {
                get { return fastCSharp.code.cSharp.tcpBase.GenericTypeServerName; }
            }
            /// <summary>
            /// TCP验证类型
            /// </summary>
            public virtual string TcpVerifyType
            {
                get { return Attribute.VerifyType.fullName(); }
            }
            /// <summary>
            /// TCP客户端验证方法类型
            /// </summary>
            public virtual string TcpVerifyMethodType
            {
                get { return Attribute.VerifyMethodType.fullName(); }
            }
            /// <summary>
            /// 是否存在泛型函数
            /// </summary>
            public bool IsAnyGenericMethod
            {
                get
                {
                    return MethodIndexs.any(value => !value.IsNullMethod && value.MemberIndex == null && value.Method.Method.IsGenericMethod);
                }
            }
            /// <summary>
            /// 是否存在验证函数
            /// </summary>
            public bool IsVerifyMethod;
            /// <summary>
            /// 客户端位置
            /// </summary>
            public string ClientPart
            {
                get { return clientPart; }
            }
            /// <summary>
            /// 用户命令起始位置
            /// </summary>
            public int CommandStartIndex
            {
                get { return fastCSharp.net.tcp.commandServer.CommandStartIndex; }
            }
            /// <summary>
            /// 最大命令长度
            /// </summary>
            public int MaxCommandLength;
            /// <summary>
            /// 服务类名称
            /// </summary>
            public abstract string ServiceName { get; }
            /// <summary>
            /// 命令序号记忆字段名称
            /// </summary>
            public string RememberIdentityCommeandName = "_identityCommandNames_";
            /// <summary>
            /// 命令序号记忆数据
            /// </summary>
            protected static readonly staticDictionary<string, int> nullRememberIdentityName = new staticDictionary<string, int>(nullValue<keyValue<string, int>>.Array);
            /// <summary>
            /// 获取命令序号记忆数据
            /// </summary>
            /// <returns></returns>
            protected staticDictionary<string, int> getRememberIdentityName()
            {
                string serverTypeName = AutoParameter.DefaultNamespace + ".tcpRemember." + ServiceName;
                Type serverType = assembly.GetType(serverTypeName);
                if (serverType != null)
                {
                    MethodInfo rememberIdentity = serverType.GetMethod(RememberIdentityCommeandName, BindingFlags.Static | BindingFlags.NonPublic);
                    if (rememberIdentity != null)
                    {
                        return new staticDictionary<string, int>(((keyValue<string, int>[])rememberIdentity.Invoke(null, null)).getFindArray(value => value.Key != null));
                    }
                }
                return nullRememberIdentityName;
            }
        }
    }
}
