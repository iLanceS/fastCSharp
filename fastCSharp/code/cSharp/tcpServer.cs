using System;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace fastCSharp.code.cSharp
{
    /// <summary>
    /// TCP服务调用配置，必须是 partial class（因为要和生成的代码共用一个类型）
    /// </summary>
    public partial class tcpServer : tcpBase
    {
        /// <summary>
        /// TCP命令服务接口
        /// </summary>
        public interface ICommandServer
        {
            /// <summary>
            /// 设置TCP命令服务端
            /// </summary>
            /// <param name="commandServer">TCP命令服务端</param>
            void SetCommandServer(fastCSharp.net.tcp.commandServer commandServer);
        }
        /// <summary>
        /// 获取TCP调用泛型函数集合
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>TCP调用泛型函数集合</returns>
        public static Dictionary<genericMethod, MethodInfo> GetGenericMethods(Type type)
        {
            if (type != null)
            {
                tcpServer tcpServer = fastCSharp.code.typeAttribute.GetAttribute<tcpServer>(type, false, false);//cSharp.Default.IsInheritAttribute
                if (tcpServer != null && tcpServer.IsSetup)
                {
                    Dictionary<genericMethod, MethodInfo> values = dictionary.Create<genericMethod, MethodInfo>();
                    code.methodInfo[] methods = code.methodInfo.GetMethods<tcpServer>(type, tcpServer.MemberFilter, false, tcpServer.IsAttribute, tcpServer.IsBaseTypeAttribute, tcpServer.IsInheritAttribute);
                    if (type.IsGenericType)
                    {
                        code.methodInfo[] definitionMethods = code.methodInfo.GetMethods<tcpServer>(type.GetGenericTypeDefinition(), tcpServer.MemberFilter, false, tcpServer.IsAttribute, tcpServer.IsBaseTypeAttribute, tcpServer.IsInheritAttribute);
                        int index = 0;
                        foreach (code.methodInfo method in methods)
                        {
                            if (method.Method.IsGenericMethod) values.Add(new genericMethod(definitionMethods[index].Method), method.Method);
                            ++index;
                        }
                    }
                    else
                    {
                        foreach (code.methodInfo method in methods)
                        {
                            if (method.Method.IsGenericMethod) values.Add(new genericMethod(method.Method), method.Method);
                        }
                    }
                    return values;
                }
            }
            return null;
        }
        /// <summary>
        /// 泛型方法调用
        /// </summary>
        /// <param name="value">服务器端目标对象</param>
        /// <param name="method">泛型方法信息</param>
        /// <param name="types">泛型参数类型集合</param>
        /// <param name="parameters">调用参数</param>
        /// <returns>返回值</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static object InvokeGenericMethod(object value, MethodInfo method, fastCSharp.code.remoteType[] types, params object[] parameters)
        {
            if (method == null) fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.Null);
            return method.MakeGenericMethod(types.getArray(type => type.Type)).Invoke(value, parameters);
        }
        /// <summary>
        /// 成员选择类型，为了防止调用者混淆了远程函数与本地函数在某些情况下产生误调用，默认只选择受保护的方法生成（包括 private / protected / internal）相关代码。
        /// </summary>
        public code.memberFilters Filter = code.memberFilters.NonPublicInstance;
        /// <summary>
        /// 成员选择类型
        /// </summary>
        public code.memberFilters MemberFilter
        {
            get { return Filter & code.memberFilters.Instance; }
        }
        /// <summary>
        /// 用于在配置文件中标识当前程序是否服务端，当在标识为服务端的环境中使用客户端调用时会输出警告日志，提示用户判断是否混淆了客户端与服务端。
        /// </summary>
        public bool IsServer;
        /// <summary>
        /// 用于给客户端生成匹配的接口类型。
        /// </summary>
        public bool IsClientInterface;
        /// <summary>
        /// 给生成的客户端添加接口类型。
        /// </summary>
        [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
        public Type ClientInterfaceType;
        /// <summary>
        /// 用于生成简单的负载均衡服务，不支持 1 问多答的 Keep 交互模式，需要自行保证写服务的幂等性，因为客户端在调用失败的时候会轮流调用不同的客户端。真实需求可能需要写个继承自 fastCSharp.net.tcp.commandClient&lt;clientType&gt;.router&lt;customType&gt; 的路由类自己实现符合需求的路由功能。
        /// </summary>
        public bool IsLoadBalancing;
        ///// <summary>
        ///// 自定义负载均衡类型(服务配置)
        ///// </summary>
        //[fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
        //public Type LoadBalancingType;
        /// <summary>
        /// 负载均衡调用错误的重复尝试次数默认为 3 次
        /// </summary>
        public int LoadBalancingTryCount = 3;
        /// <summary>
        /// 负载均衡保持连接心跳包间隔时间默认为 2 秒。
        /// </summary>
        public int LoadBalancingCheckSeconds;
        /// <summary>
        /// 负载均衡路由创建客户端失败重试间隔秒数，默认为 0 表示不重试。
        /// </summary>
        public int LoadBalancingRouterRetrySeconds;
#if MONO
        ///// <summary>
        ///// 客户端队列初始容器大小(2^n)
        ///// </summary>
        //public byte AcceptQueueSize = 4;
#endif
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="type">TCP服务器类型</param>
        /// <returns>TCP调用服务器端配置信息</returns>
        public static tcpServer GetConfig(Type type)
        {
            tcpServer attribute = fastCSharp.code.typeAttribute.GetAttribute<tcpServer>(type, false, true);
            return attribute != null ? fastCSharp.config.pub.LoadConfig(attribute.Clone(), attribute.Service) : null;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="serviceName">TCP调用服务名称</param>
        /// <param name="type">TCP服务器类型</param>
        /// <returns>TCP调用服务器端配置信息</returns>
        public static tcpServer GetConfig(string serviceName, Type type = null)
        {
            tcpServer attribute = null;
            if (type != null)
            {
                attribute = fastCSharp.code.typeAttribute.GetAttribute<tcpServer>(type, false, true);
                if (attribute != null) attribute = attribute.Clone();
            }
            attribute = fastCSharp.config.pub.LoadConfig(attribute ?? new tcpServer(), serviceName);
            if (attribute.Service == null) attribute.Service = serviceName;
            return attribute;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="serviceName">TCP调用服务名称</param>
        /// <param name="type">TCP服务器类型</param>
        /// <returns>TCP调用服务器端配置信息</returns>
        public static tcpServer GetTcpCallConfig(string serviceName, Type type = null)
        {
            tcpServer attribute = new tcpServer();
            if (type != null)
            {
                tcpCall tcpCall = fastCSharp.code.typeAttribute.GetAttribute<tcpCall>(type, false, true);
                if (tcpCall != null) attribute.CopyFrom(tcpCall);
            }
            attribute = fastCSharp.config.pub.LoadConfig(attribute, serviceName);
            if (attribute.Service == null) attribute.Service = serviceName;
            return attribute;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="attribute">TCP服务器配置信息</param>
        /// <param name="serviceName">TCP调用服务名称</param>
        /// <returns>TCP调用服务器端配置信息</returns>
        public static tcpServer GetConfig(tcpServer attribute, string serviceName)
        {
            attribute = fastCSharp.config.pub.LoadConfig(attribute, serviceName);
            if (attribute.Service == null) attribute.Service = serviceName;
            return attribute;
        }
        /// <summary>
        /// 获取TcpCall客户端虚拟配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static tcpServer GetTcpCallConfig(Type type)
        {
            fastCSharp.code.cSharp.tcpCall tcpCall = fastCSharp.code.typeAttribute.GetAttribute<fastCSharp.code.cSharp.tcpCall>(type, false, true);
            if (tcpCall == null) return null;
            fastCSharp.code.cSharp.tcpServer attribute = tcpCall.Copy<fastCSharp.code.cSharp.tcpServer, fastCSharp.code.cSharp.tcpBase>();
            attribute = fastCSharp.config.pub.LoadConfig(attribute, attribute.Service = tcpCall.ServiceName);
            attribute.TcpRegister = null;
            attribute.IsServer = true;
            return attribute;
        }
    }
}
