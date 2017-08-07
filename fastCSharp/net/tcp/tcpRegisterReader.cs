using System;
using System.Threading;
using System.Runtime.CompilerServices;
using fastCSharp.threading;
using System.Collections.Generic;
using System.IO;

namespace fastCSharp.net.tcp
{
    /// <summary>
    /// TCP注册服务
    /// </summary>
#if NotFastCSharpCode
    [fastCSharp.code.cSharp.tcpServer(Service = tcpRegisterReader.ServiceName, IsIdentityCommand = true)]
#else
    [fastCSharp.code.cSharp.tcpServer(Service = tcpRegisterReader.ServiceName, IsIdentityCommand = true, VerifyMethodType = typeof(tcpRegisterReader.tcpClient.timeVerifyMethod))]
#endif
    public partial class tcpRegisterReader : timeVerifyServer
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        internal const string ServiceName = "tcpRegisterReader";
        /// <summary>
        /// TCP服务信息
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct service
        {
            /// <summary>
            /// TCP服务名称标识
            /// </summary>
            public string Name;
            /// <summary>
            /// 端口信息集合
            /// </summary>
            public host Host;
            /// <summary>
            /// 是否检测Host冲突
            /// </summary>
            public bool IsCheck;
            /// <summary>
            /// 是否只允许一个TCP服务实例
            /// </summary>
            public bool IsSingle;
            /// <summary>
            /// 是否预申请服务
            /// </summary>
            public bool IsPerp;
        }
        /// <summary>
        /// TCP服务端标识
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public struct clientId
        {
            /// <summary>
            /// 日志流时钟周期标识
            /// </summary>
            public long Tick;
            /// <summary>
            /// 客户端池索引标识
            /// </summary>
            public indexIdentity Identity;
        }
        /// <summary>
        /// 日志
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public sealed class log
        {
            /// <summary>
            /// 日志类型
            /// </summary>
            public enum type
            {
                /// <summary>
                /// 客户端错误
                /// </summary>
                ClientError,
                /// <summary>
                /// 回调测试
                /// </summary>
                Check,
                /// <summary>
                /// 重新获取日志
                /// </summary>
                NewGetter,
                ///// <summary>
                ///// 版本错误
                ///// </summary>
                //VersionError,

                /// <summary>
                /// TCP服务端口变化
                /// </summary>
                HostChanged,
                /// <summary>
                /// 删除服务信息
                /// </summary>
                RemoveServiceName,
            }
            /// <summary>
            /// 日志类型
            /// </summary>
            public type Type;
            /// <summary>
            /// TCP服务信息集合
            /// </summary>
            public services Services;

            /// <summary>
            /// 客户端错误
            /// </summary>
            public static readonly log ClientError = new log { Type = type.ClientError };
            /// <summary>
            /// 回调测试
            /// </summary>
            public static readonly log Check = new log { Type = type.Check };
            /// <summary>
            /// 重新获取日志
            /// </summary>
            public static readonly log NewGetter = new log { Type = type.NewGetter };
            ///// <summary>
            ///// 客户端错误
            ///// </summary>
            //public static readonly log VersionError = new log { Type = type.VersionError };
        }
        /// <summary>
        /// TCP服务信息集合
        /// </summary>
        [fastCSharp.emit.dataSerialize(IsReferenceMember = false, IsMemberMap = false)]
        public sealed class services
        {
            /// <summary>
            /// 最后一次未找到注册的服务名称
            /// </summary>
            private static string errorServiceName;
            /// <summary>
            /// TCP服务名称标识
            /// </summary>
            public string Name;
            /// <summary>
            /// 端口信息集合
            /// </summary>
            public host[] Hosts;
            /// <summary>
            /// 预备服务集合
            /// </summary>
            private subArray<keyValue<indexIdentity, service>> perpServices;
            /// <summary>
            /// 是否只允许一个TCP服务实例
            /// </summary>
            public bool IsSingle;
            /// <summary>
            /// 预备服务是否只允许一个TCP服务实例
            /// </summary>
            private bool isPerpSingle;
            /// <summary>
            /// 设置预备服务
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="service"></param>
            internal void SetPerpService(indexIdentity identity, service service)
            {
                if (isPerpSingle || service.IsSingle)
                {
                    perpServices.Empty();
                    perpServices.Add(new keyValue<indexIdentity, service>(identity, service));
                    isPerpSingle = service.IsSingle;
                }
                else perpServices.Add(new keyValue<indexIdentity, service>(identity, service));
            }
            /// <summary>
            /// 删除预备服务
            /// </summary>
            /// <param name="identity"></param>
            internal void RemovePerpService(indexIdentity identity)
            {
                if (perpServices.length != 0)
                {
                    int count = perpServices.length;
                    foreach (keyValue<indexIdentity, service> service in perpServices.array)
                    {
                        if (service.Key.Equals(identity) == 0)
                        {
                            perpServices.RemoveAt(perpServices.length - count);
                            break;
                        }
                        if (--count == 0) break;
                    }
                }
            }
            /// <summary>
            /// 获取预备服务
            /// </summary>
            /// <returns></returns>
            internal subArray<keyValue<indexIdentity, service>> GetPerpServices()
            {
                subArray<keyValue<indexIdentity, service>> services = perpServices;
                perpServices.Null();
                return services;
            }

            /// <summary>
            /// TCP调用客户端集合
            /// </summary>
            [fastCSharp.code.ignore]
            internal list<commandClient> Clients;
            ///// <summary>
            ///// 端口信息更新版本
            ///// </summary>
            //[fastCSharp.code.ignore]
            //internal int Version;
            /// <summary>
            /// 当前端口信息位置
            /// </summary>
            [fastCSharp.code.ignore]
            private int hostIndex;
            /// <summary>
            /// 复制TCP服务信息
            /// </summary>
            /// <param name="services">TCP服务信息集合</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Copy(services services)
            {
                Hosts = services.Hosts;
                IsSingle = services.IsSingle;
                hostIndex = 0;
                //++Version;
                changeTcpRegisterServicesVersion();
            }
            /// <summary>
            /// 清空数据
            /// </summary>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void Remove()
            {
                if (Hosts.Length != 0)
                {
                    Hosts = nullValue<host>.Array;
                    hostIndex = 0;
                    //++Version;
                    changeTcpRegisterServicesVersion();
                    //return true;
                }
                //return false;
            }
            /// <summary>
            /// 设置端口信息集合
            /// </summary>
            /// <param name="hosts"></param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void SetHosts(host[] hosts)
            {
                Hosts = hosts;
                hostIndex = 0;
                //++Version;
                changeTcpRegisterServicesVersion();
            }

            /// <summary>
            /// 获取TCP服务端口信息
            /// </summary>
            /// <param name="commandClient">TCP调用客户端</param>
            /// <returns>是否更新</returns>
            internal bool GetHost(commandClient commandClient)
            {
                fastCSharp.code.cSharp.tcpServer attribute = commandClient.Attribute;
                if (attribute != null)
                {
                    //commandClient.TcpRegisterServicesVersion = Version;
                    if (Hosts.Length == 0)
                    {
                        attribute.Port = 0;
                        if (errorServiceName != attribute.ServiceName) fastCSharp.log.Error.Add(attribute.ServiceName + "[" + attribute.TcpRegisterName + "] 未找到注册服务信息", new System.Diagnostics.StackFrame(), false);
                        errorServiceName = attribute.ServiceName;
                    }
                    else
                    {
                        if (errorServiceName == attribute.ServiceName) errorServiceName = null;
                        host host;
                        int index = hostIndex;
                        if (index < Hosts.Length)
                        {
                            ++hostIndex;
                            host = Hosts[index];
                        }
                        else
                        {
                            hostIndex = 1;
                            host = Hosts[0];
                        }
                        if (attribute.IsFixedClientHost)
                        {
                            if (attribute.Port != host.Port) attribute.Port = host.Port;
                        }
                        else if (attribute.Host != host.Host || attribute.Port != host.Port)
                        {
                            attribute.Host = host.Host;
                            attribute.Port = host.Port;
                        }
                    }
                    return true;
                }
                return false;
            }
            /// <summary>
            /// TCP服务信息集合版本变化处理
            /// </summary>
            private void changeTcpRegisterServicesVersion()
            {
                if (Clients != null)
                {
                    foreach (commandClient client in Clients) client.ChangeTcpRegisterServicesVersion();
                }
            }
            /// <summary>
            /// 添加TCP调用客户端
            /// </summary>
            /// <param name="client">TCP调用客户端</param>
            [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
            internal void AddClient(commandClient client)
            {
                if (Clients == null) Clients = new list<commandClient>(sizeof(int));
                Clients.Add(client);
                client.ChangeTcpRegisterServicesVersion();
            }
            /// <summary>
            /// 删除TCP调用客户端
            /// </summary>
            /// <param name="removeClient">TCP调用客户端</param>
            internal void RemoveClient(commandClient removeClient)
            {
                if (Clients != null && Clients.length != 0)
                {
                    int count = Clients.length;
                    commandClient[] clientArray = Clients.array;
                    foreach (commandClient client in clientArray)
                    {
                        if (client == removeClient)
                        {
                            count = Clients.length - count;
                            Clients.UnsafeAddLength(-1);
                            clientArray[count] = clientArray[Clients.length];
                            clientArray[Clients.length] = null;
                            return;
                        }
                        if (--count == 0) break;
                    }
                }
            }
            /// <summary>
            /// 默认空TCP服务信息集合
            /// </summary>
            internal static readonly services Null = new services();
        }

        /// <summary>
        /// TCP注册服务
        /// </summary>
        public tcpRegister TcpRegister { get; private set; }
        /// <summary>
        /// TCP服务端注册
        /// </summary>
        /// <returns>TCP服务端标识</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsServerSynchronousTask = false, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private clientId register()
        {
            return TcpRegister.Register();
        }
        /// <summary>
        /// 获取TCP服务信息集合
        /// </summary>
        /// <param name="logIdentity">日志流编号</param>
        /// <returns>TCP服务信息集合</returns>
        [fastCSharp.code.cSharp.tcpMethod(IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private services[] getServices(out int logIdentity)
        {
            return TcpRegister.GetServices(out logIdentity);
        }
        /// <summary>
        /// TCP服务端轮询
        /// </summary>
        /// <param name="clientId">TCP服务端标识</param>
        /// <param name="logIdentity">日志编号</param>
        /// <param name="onLog">TCP服务注册通知委托</param>
        [fastCSharp.code.cSharp.tcpMethod(IsClientAsynchronous = true, IsClientSynchronous = false, IsKeepCallback = true, IsInputSerializeReferenceMember = false, IsOutputSerializeReferenceMember = false)]
        private void getLog(clientId clientId, int logIdentity, Func<fastCSharp.net.returnValue<log>, bool> onLog)
        {
            TcpRegister.GetLog(ref clientId, logIdentity, onLog);
        }
        /// <summary>
        /// TCP注册服务
        /// </summary>
        /// <returns></returns>
        public static tcpRegisterReader Create()
        {
            return new tcpRegisterReader { TcpRegister = new tcpRegister() };
        }
    }
}
