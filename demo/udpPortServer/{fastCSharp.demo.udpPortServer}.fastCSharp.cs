//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.udpPortServer
{
        public partial class portServer
        {

            /// <summary>
            /// portServer TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.demo.udpPortServer.portServer _value_;
                /// <summary>
                /// portServer TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null, fastCSharp.demo.udpPortServer.portServer value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("portServer", typeof(fastCSharp.demo.udpPortServer.portServer)), verify)
                {
                    _value_ = value ?? new fastCSharp.demo.udpPortServer.portServer();
                    setCommands(1);
                    identityOnCommands[0 + 128].Set(0 + 128, 256);
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < 128) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - 128)
                        {
                            case 0: _M0(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                internal struct _i0
                {
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer name;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.udpPortServer.ipPort>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.udpPortServer.ipPort Ret;
                    public fastCSharp.demo.udpPortServer.ipPort Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.udpPortServer.ipPort)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.demo.udpPortServer.portServer, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            fastCSharp.demo.udpPortServer.ipPort Return;


                            
                            Return = serverValue.get(inputParameter.name);

                            value.Value = new _o0
                            {
                                Return = Return
                            };
                            value.Type = fastCSharp.net.returnValue.type.Success;
                        }
                        catch (Exception error)
                        {
                            value.Type = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    }
                    public override void Call()
                    {
                        fastCSharp.net.returnValue<_o0> value = new fastCSharp.net.returnValue<_o0>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s0>.PushNotNull(this);
                    }
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i0 inputParameter = new _i0();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, _value_, ref inputParameter ));
                                return;
                            }
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                private fastCSharp.net.tcp.commandClient tcpCommandClient;
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                public fastCSharp.net.tcp.commandClient TcpCommandClient { get { return tcpCommandClient; } }
                /// <summary>
                /// TCP调用客户端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("portServer", typeof(fastCSharp.demo.udpPortServer.portServer)), 28, verify);
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }


                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                public fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
                {
                    fastCSharp.net.tcp.commandClient client = TcpCommandClient;
                    return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
                }

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 256, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<fastCSharp.demo.udpPortServer.ipPort> get(fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer name)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.get(name, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<fastCSharp.demo.udpPortServer.ipPort>{ Type = _returnType_ };
                }


                private void get(fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer name, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                name = name,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c0, ref _inputParameter_, ref _returnType_.Value, _isTask_);
                            return;
                        }
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientDisposed;
                    }
                    catch (Exception _error_)
                    {
                        _returnType_.Type = fastCSharp.net.returnValue.type.ClientException;
                        fastCSharp.log.Error.Add(_error_, null, false);
                    }
                    if (_callback_ != null) _callback_.Callback(ref _returnType_);
                    else if (_onReturn_ != null) _onReturn_(_returnType_);
                }
            }

        }
}
#endif