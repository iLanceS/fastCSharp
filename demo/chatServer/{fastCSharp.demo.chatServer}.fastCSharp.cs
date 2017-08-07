//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.chatServer
{
        public partial class server
        {

            /// <summary>
            ///  TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.demo.chatServer.server _value_;
                /// <summary>
                ///  TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null, fastCSharp.demo.chatServer.server value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.demo.chatServer.server)), verify)
                {
                    _value_ = value ?? new fastCSharp.demo.chatServer.server();
                    setCommands(5);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128, 0);
                    identityOnCommands[2 + 128].Set(2 + 128, 0);
                    identityOnCommands[3 + 128].Set(3 + 128);
                    identityOnCommands[4 + 128].Set(4 + 128, 0);
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
                            case 1: _M1(socket, ref data); return;
                            case 2: _M2(socket, ref data); return;
                            case 3: _M3(socket, ref data); return;
                            case 4: _M4(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public string user;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public bool Ret;
                    public bool Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (bool)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.demo.chatServer.server, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.login(socket, inputParameter.user);

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
                                _s0/**/.Call(socket, _value_, ref inputParameter );
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
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.demo.chatServer.server>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.logout(socket);

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
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s1>.PushNotNull(this);
                    }
                }
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                _s1/**/.Call(socket, _value_ );
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<string[]>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public string[] Ret;
                    public string[] Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (string[])value; }
                    }
#endif
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                _o2 outputParameter = new _o2();
                                Func<fastCSharp.net.returnValue<string[]>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o2, string[]>.GetKeep(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.getUsers(socket, callbackReturn);
                                }
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                    socket.SendStream(socket.Identity, returnType);
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i3
                {
                    public string message;
                    public string[] users;
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.demo.chatServer.server, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.send(socket, inputParameter.message, inputParameter.users);

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
                        fastCSharp.net.returnValue value = new fastCSharp.net.returnValue();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s3>.PushNotNull(this);
                    }
                }
                private void _M3(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i3 inputParameter = new _i3();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s3/**/.Call(socket, _value_, ref inputParameter );
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
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.chatServer.server.message>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.chatServer.server.message Ret;
                    public fastCSharp.demo.chatServer.server.message Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.chatServer.server.message)value; }
                    }
#endif
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                _o4 outputParameter = new _o4();
                                Func<fastCSharp.net.returnValue<fastCSharp.demo.chatServer.server.message>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o4, fastCSharp.demo.chatServer.server.message>.GetKeep(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.receive(socket, callbackReturn);
                                }
                                return;
                            }
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
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("", typeof(fastCSharp.demo.chatServer.server)), 28, verify);
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

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> login(string user)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.login(user, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void login(string user, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                user = user,
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue logout()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.logout(null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void logout(Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    logout(_onReturn_, null, true);
                }
                private void logout(Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Call(_onReturn_, _callback_, _c1, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 0, IsKeepCallback = 1, IsSendOnly = 0 };

                /// <returns>保持异步回调</returns>
                public fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback getUsers(Action<fastCSharp.net.returnValue<string[]>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<string[], tcpServer._o2>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        
                        return getUsers(null, _onOutput_, true);
                    }
                    return null;
                }


                private fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback getUsers(Action<fastCSharp.net.returnValue<tcpServer._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o2> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            return _socket_.Get(_onReturn_, _callback_, _c2, ref _returnType_.Value, _isTask_);
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
                    return null;
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue send(string message, string[] users)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.send(message, users, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void send(string message, string[] users, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    send(message, users, _onReturn_, null, true);
                }
                private void send(string message, string[] users, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                message = message,
                                users = users,
                            };
                            _socket_.Call<tcpServer._i3>(_onReturn_, _callback_, _c3, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 0, IsKeepCallback = 1, IsSendOnly = 0 };

                /// <returns>保持异步回调</returns>
                public fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback receive(Action<fastCSharp.net.returnValue<fastCSharp.demo.chatServer.server.message>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.chatServer.server.message, tcpServer._o4>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        
                        return receive(null, _onOutput_, true);
                    }
                    return null;
                }


                private fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback receive(Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            return _socket_.Get(_onReturn_, _callback_, _c4, ref _returnType_.Value, _isTask_);
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
                    return null;
                }
            }

        }
}
#endif