//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.diagnostics
{
        public partial class processCopyServer : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {

            /// <summary>
            /// processCopy TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.diagnostics.processCopyServer _value_;
                /// <summary>
                /// processCopy TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.diagnostics.processCopyServer value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("processCopy", typeof(fastCSharp.diagnostics.processCopyServer)))
                {
                    _value_ = value ?? new fastCSharp.diagnostics.processCopyServer();
                    setCommands(4);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[verifyCommandIdentity = 3 + 128].Set(3 + 128, 1024);
                    _value_.SetCommandServer(this);
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
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public fastCSharp.diagnostics.processCopyServer.copyer copyer;
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.diagnostics.processCopyServer, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.copyStart(inputParameter.copyer);

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
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i1
                {
                    public fastCSharp.diagnostics.processCopyServer.copyer copyer;
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.diagnostics.processCopyServer, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.guard(inputParameter.copyer);

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
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i2
                {
                    public fastCSharp.diagnostics.processCopyServer.copyer copyer;
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.diagnostics.processCopyServer, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.remove(inputParameter.copyer);

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
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i3
                {
                    public ulong randomPrefix;
                    public byte[] md5Data;
                    public long ticks;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public long ticks;
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
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.diagnostics.processCopyServer, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o3
                            {
                                ticks = inputParameter.ticks,
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
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
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
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("processCopy", typeof(fastCSharp.diagnostics.processCopyServer)), 28, verifyMethod ?? new fastCSharp.diagnostics.processCopyServer.tcpClient.timeVerifyMethod(), this);
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

                /// <summary>
                /// 复制重启进程
                /// </summary>
                /// <param name="copyer">文件复制器</param>
                public fastCSharp.net.returnValue copyStart(fastCSharp.diagnostics.processCopyServer.copyer copyer)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.copyStart(copyer, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 复制重启进程
                /// </summary>
                /// <param name="copyer">文件复制器</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void copyStart(fastCSharp.diagnostics.processCopyServer.copyer copyer, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    copyStart(copyer, _onReturn_, null, true);
                }
                private void copyStart(fastCSharp.diagnostics.processCopyServer.copyer copyer, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                copyer = copyer,
                            };
                            _socket_.Call<tcpServer._i0>(_onReturn_, _callback_, _c0, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 守护进程
                /// </summary>
                /// <param name="copyer">文件信息</param>
                public fastCSharp.net.returnValue guard(fastCSharp.diagnostics.processCopyServer.copyer copyer)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.guard(copyer, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 守护进程
                /// </summary>
                /// <param name="copyer">文件信息</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void guard(fastCSharp.diagnostics.processCopyServer.copyer copyer, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    guard(copyer, _onReturn_, null, true);
                }
                private void guard(fastCSharp.diagnostics.processCopyServer.copyer copyer, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                copyer = copyer,
                            };
                            _socket_.Call<tcpServer._i1>(_onReturn_, _callback_, _c1, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 删除守护进程
                /// </summary>
                /// <param name="copyer">文件信息</param>
                public fastCSharp.net.returnValue remove(fastCSharp.diagnostics.processCopyServer.copyer copyer)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.remove(copyer, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 删除守护进程
                /// </summary>
                /// <param name="copyer">文件信息</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void remove(fastCSharp.diagnostics.processCopyServer.copyer copyer, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    remove(copyer, _onReturn_, null, true);
                }
                private void remove(fastCSharp.diagnostics.processCopyServer.copyer copyer, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                copyer = copyer,
                            };
                            _socket_.Call<tcpServer._i2>(_onReturn_, _callback_, _c2, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 时间验证函数
                /// </summary>
                /// <returns>是否验证成功</returns>
                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o3> _wait_ = fastCSharp.net.waitCall<tcpServer._o3>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            ticks = _outputParameter_.Value.ticks;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
}namespace fastCSharp.io
{
        public partial class fileBlockServer : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {

            /// <summary>
            /// fileBlock TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.io.fileBlockServer _value_;
                /// <summary>
                /// fileBlock TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.io.fileBlockServer value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("fileBlock", typeof(fastCSharp.io.fileBlockServer)))
                {
                    _value_ = value ?? new fastCSharp.io.fileBlockServer();
                    setCommands(6);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[3 + 128].Set(3 + 128, 0);
                    identityOnCommands[4 + 128].Set(4 + 128);
                    identityOnCommands[verifyCommandIdentity = 5 + 128].Set(5 + 128, 1024);
                    _value_.SetCommandServer(this);
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
                            case 5: _M5(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayEvent buffer;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayEvent Ret;
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayEvent Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.code.cSharp.tcpBase.subByteArrayEvent)value; }
                    }
#endif
                }
                private void _M0(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i0 inputParameter = new _i0();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _o0 outputParameter = new _o0();
                                Func<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o0, fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>.Get(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.read(inputParameter.buffer, callbackReturn);
                                }
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
                internal struct _i1
                {
                    public fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<long>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public long Ret;
                    public long Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (long)value; }
                    }
#endif
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.io.fileBlockServer, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            long Return;


                            
                            Return = serverValue.writeSynchronous(inputParameter.dataStream);

                            value.Value = new _o1
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
                        fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
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
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s1/**/.Call(socket, _value_, ref inputParameter );
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
                internal struct _i2
                {
                    public fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<long>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public long Ret;
                    public long Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (long)value; }
                    }
#endif
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _o2 outputParameter = new _o2();
                                Func<fastCSharp.net.returnValue<long>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o2, long>.Get(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.write(inputParameter.dataStream, callbackReturn);
                                }
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
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.io.fileBlockServer>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.waitBuffer();

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
                            {
                                fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, _value_ ));
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
                internal struct _i4
                {
                    public bool isDiskFile;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.io.fileBlockServer, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.flush(inputParameter.isDiskFile);

                            value.Value = new _o4
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
                        fastCSharp.net.returnValue<_o4> value = new fastCSharp.net.returnValue<_o4>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s4>.PushNotNull(this);
                    }
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i4 inputParameter = new _i4();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s4/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i5
                {
                    public ulong randomPrefix;
                    public byte[] md5Data;
                    public long ticks;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public long ticks;
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
                sealed class _s5 : fastCSharp.net.tcp.commandServer.serverCall<_s5, fastCSharp.io.fileBlockServer, _i5>
                {
                    private void get(ref fastCSharp.net.returnValue<_o5> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o5
                            {
                                ticks = inputParameter.ticks,
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
                        fastCSharp.net.returnValue<_o5> value = new fastCSharp.net.returnValue<_o5>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s5>.PushNotNull(this);
                    }
                }
                private void _M5(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i5 inputParameter = new _i5();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s5/**/.Call(socket, _value_, ref inputParameter );
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
            public class tcpClient : fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("fileBlock", typeof(fastCSharp.io.fileBlockServer)), 28, verifyMethod ?? new fastCSharp.io.fileBlockServer.tcpClient.timeVerifyMethod(), this);
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

                /// <summary>
                /// 读取文件分块数据
                /// </summary>
                /// <param name="buffer">数据缓冲区</param>
                public fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent> read(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent buffer)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.read(buffer, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayEvent>{ Type = _returnType_ };
                }


                private void read(fastCSharp.code.cSharp.tcpBase.subByteArrayEvent buffer, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                buffer = buffer,
                            };
                            _returnType_.Value.Ret = _inputParameter_.buffer;
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写入文件分块数据(单客户端独占操作)
                /// </summary>
                /// <param name="dataStream">文件分块数据</param>
                /// <returns>写入文件位置</returns>
                public fastCSharp.net.returnValue<long> writeSynchronous(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o1> _wait_ = fastCSharp.net.waitCall<tcpServer._o1>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.writeSynchronous(dataStream, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<long>{ Type = _returnType_ };
                }
                /// <summary>
                /// 写入文件分块数据(单客户端独占操作)
                /// </summary>
                /// <param name="dataStream">文件分块数据</param>
                /// <param name="_onReturn_">写入文件位置</param>
                public void writeSynchronous(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream, Action<fastCSharp.net.returnValue<long>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<long, tcpServer._o1>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        writeSynchronous(dataStream, null, _onOutput_, false);
                    }
                }


                private void writeSynchronous(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream, Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                dataStream = dataStream,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写入文件分块数据(多客户端并行操作)
                /// </summary>
                /// <param name="dataStream">文件分块数据</param>
                public fastCSharp.net.returnValue<long> write(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o2> _wait_ = fastCSharp.net.waitCall<tcpServer._o2>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.write(dataStream, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o2> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<long>{ Type = _returnType_ };
                }


                private void write(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream, Action<fastCSharp.net.returnValue<tcpServer._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o2> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                dataStream = dataStream,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c2, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 等待缓存写入
                /// </summary>
                public fastCSharp.net.returnValue waitBuffer()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.waitBuffer(null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 等待缓存写入
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void waitBuffer(Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    waitBuffer(_onReturn_, null, true);
                }
                private void waitBuffer(Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Call(_onReturn_, _callback_, _c3, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写入缓存
                /// </summary>
                /// <param name="isDiskFile">是否写入到磁盘文件</param>
                /// <returns>是否成功</returns>
                public fastCSharp.net.returnValue<bool> flush(bool isDiskFile)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o4> _wait_ = fastCSharp.net.waitCall<tcpServer._o4>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.flush(isDiskFile, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void flush(bool isDiskFile, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                isDiskFile = isDiskFile,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c5 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 5 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o5> _wait_ = fastCSharp.net.waitCall<tcpServer._o5>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o5> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            ticks = _outputParameter_.Value.ticks;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<tcpServer._o5>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o5>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o5> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o5>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i5 _inputParameter_ = new tcpServer._i5
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c5, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
}namespace fastCSharp.memoryDatabase
{
        public partial class physicalServer : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {

            /// <summary>
            /// memoryDatabasePhysical TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.memoryDatabase.physicalServer _value_;
                /// <summary>
                /// memoryDatabasePhysical TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.memoryDatabase.physicalServer value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("memoryDatabasePhysical", typeof(fastCSharp.memoryDatabase.physicalServer)))
                {
                    _value_ = value ?? new fastCSharp.memoryDatabase.physicalServer();
                    setCommands(11);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[3 + 128].Set(3 + 128);
                    identityOnCommands[4 + 128].Set(4 + 128);
                    identityOnCommands[5 + 128].Set(5 + 128);
                    identityOnCommands[6 + 128].Set(6 + 128);
                    identityOnCommands[7 + 128].Set(7 + 128);
                    identityOnCommands[8 + 128].Set(8 + 128);
                    identityOnCommands[9 + 128].Set(9 + 128);
                    identityOnCommands[verifyCommandIdentity = 10 + 128].Set(10 + 128, 1024);
                    _value_.SetCommandServer(this);
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
                            case 5: _M5(socket, ref data); return;
                            case 6: _M6(socket, ref data); return;
                            case 7: _M7(socket, ref data); return;
                            case 8: _M8(socket, ref data); return;
                            case 9: _M9(socket, ref data); return;
                            case 10: _M10(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public string fileName;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.memoryDatabase.physicalServer.physicalIdentity>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.memoryDatabase.physicalServer.physicalIdentity Ret;
                    public fastCSharp.memoryDatabase.physicalServer.physicalIdentity Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.memoryDatabase.physicalServer.physicalIdentity)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.memoryDatabase.physicalServer, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            fastCSharp.memoryDatabase.physicalServer.physicalIdentity Return;


                            
                            Return = serverValue.open(inputParameter.fileName);

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
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i1
                {
                    public fastCSharp.memoryDatabase.physicalServer.timeIdentity identity;
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.memoryDatabase.physicalServer, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.close(inputParameter.identity);

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
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s1/**/.Call(socket, _value_, ref inputParameter );
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
                internal struct _i2
                {
                    public fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream stream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.memoryDatabase.physicalServer, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue<_o2> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.create(inputParameter.stream);

                            value.Value = new _o2
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
                        fastCSharp.net.returnValue<_o2> value = new fastCSharp.net.returnValue<_o2>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s2/**/.Call(socket, _value_, ref inputParameter );
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
                internal struct _i3
                {
                    public fastCSharp.memoryDatabase.physicalServer.timeIdentity identity;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer Ret;
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer)value; }
                    }
#endif
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.memoryDatabase.physicalServer, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer Return;


                            
                            Return = serverValue.loadHeader(inputParameter.identity);

                            value.Value = new _o3
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
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
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
                                fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i4
                {
                    public fastCSharp.memoryDatabase.physicalServer.timeIdentity identity;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer Ret;
                    public fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer)value; }
                    }
#endif
                }
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.memoryDatabase.physicalServer, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer Return;


                            
                            Return = serverValue.load(inputParameter.identity);

                            value.Value = new _o4
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
                        fastCSharp.net.returnValue<_o4> value = new fastCSharp.net.returnValue<_o4>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s4>.PushNotNull(this);
                    }
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i4 inputParameter = new _i4();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s4/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i5
                {
                    public fastCSharp.memoryDatabase.physicalServer.timeIdentity identity;
                    public bool isLoaded;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
                sealed class _s5 : fastCSharp.net.tcp.commandServer.serverCall<_s5, fastCSharp.memoryDatabase.physicalServer, _i5>
                {
                    private void get(ref fastCSharp.net.returnValue<_o5> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.loaded(inputParameter.identity, inputParameter.isLoaded);

                            value.Value = new _o5
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
                        fastCSharp.net.returnValue<_o5> value = new fastCSharp.net.returnValue<_o5>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s5>.PushNotNull(this);
                    }
                }
                private void _M5(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i5 inputParameter = new _i5();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s5/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i6
                {
                    public fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public int Ret;
                    public int Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (int)value; }
                    }
#endif
                }
                sealed class _s6 : fastCSharp.net.tcp.commandServer.serverCall<_s6, fastCSharp.memoryDatabase.physicalServer, _i6>
                {
                    private void get(ref fastCSharp.net.returnValue<_o6> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.append(inputParameter.dataStream);

                            value.Value = new _o6
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
                        fastCSharp.net.returnValue<_o6> value = new fastCSharp.net.returnValue<_o6>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s6>.PushNotNull(this);
                    }
                }
                private void _M6(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i6 inputParameter = new _i6();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s6/**/.Call(socket, _value_, ref inputParameter );
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
                internal struct _i7
                {
                    public fastCSharp.memoryDatabase.physicalServer.timeIdentity identity;
                }
                sealed class _s7 : fastCSharp.net.tcp.commandServer.serverCall<_s7, fastCSharp.memoryDatabase.physicalServer, _i7>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.waitBuffer(inputParameter.identity);

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
                        fastCSharp.typePool<_s7>.PushNotNull(this);
                    }
                }
                private void _M7(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i7 inputParameter = new _i7();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s7/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i8
                {
                    public fastCSharp.memoryDatabase.physicalServer.timeIdentity identity;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
                sealed class _s8 : fastCSharp.net.tcp.commandServer.serverCall<_s8, fastCSharp.memoryDatabase.physicalServer, _i8>
                {
                    private void get(ref fastCSharp.net.returnValue<_o8> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.flush(inputParameter.identity);

                            value.Value = new _o8
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
                        fastCSharp.net.returnValue<_o8> value = new fastCSharp.net.returnValue<_o8>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s8>.PushNotNull(this);
                    }
                }
                private void _M8(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i8 inputParameter = new _i8();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s8/**/.Call(socket, _value_, ref inputParameter );
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
                internal struct _i9
                {
                    public fastCSharp.memoryDatabase.physicalServer.timeIdentity identity;
                    public bool isDiskFile;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o9 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o9 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
                sealed class _s9 : fastCSharp.net.tcp.commandServer.serverCall<_s9, fastCSharp.memoryDatabase.physicalServer, _i9>
                {
                    private void get(ref fastCSharp.net.returnValue<_o9> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.flushFile(inputParameter.identity, inputParameter.isDiskFile);

                            value.Value = new _o9
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
                        fastCSharp.net.returnValue<_o9> value = new fastCSharp.net.returnValue<_o9>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s9>.PushNotNull(this);
                    }
                }
                private void _M9(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i9 inputParameter = new _i9();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s9/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i10
                {
                    public ulong randomPrefix;
                    public byte[] md5Data;
                    public long ticks;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o10 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o10 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public long ticks;
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
                sealed class _s10 : fastCSharp.net.tcp.commandServer.serverCall<_s10, fastCSharp.memoryDatabase.physicalServer, _i10>
                {
                    private void get(ref fastCSharp.net.returnValue<_o10> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o10
                            {
                                ticks = inputParameter.ticks,
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
                        fastCSharp.net.returnValue<_o10> value = new fastCSharp.net.returnValue<_o10>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s10>.PushNotNull(this);
                    }
                }
                private void _M10(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i10 inputParameter = new _i10();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s10/**/.Call(socket, _value_, ref inputParameter );
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
            public class tcpClient : fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("memoryDatabasePhysical", typeof(fastCSharp.memoryDatabase.physicalServer)), 28, verifyMethod ?? new fastCSharp.memoryDatabase.physicalServer.tcpClient.timeVerifyMethod(), this);
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

                /// <summary>
                /// 打开数据库
                /// </summary>
                /// <param name="fileName">数据文件名</param>
                /// <param name="_onReturn_">数据库物理层初始化信息</param>
                public void open(string fileName, Action<fastCSharp.net.returnValue<fastCSharp.memoryDatabase.physicalServer.physicalIdentity>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.memoryDatabase.physicalServer.physicalIdentity, tcpServer._o0>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        open(fileName, null, _onOutput_, true);
                    }
                }


                private void open(string fileName, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                fileName = fileName,
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 关闭数据库文件
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                public fastCSharp.net.returnValue close(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.close(identity, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 关闭数据库文件
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void close(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    close(identity, _onReturn_, null, true);
                }
                private void close(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                identity = identity,
                            };
                            _socket_.Call<tcpServer._i1>(_onReturn_, _callback_, _c1, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 创建数据库文件
                /// </summary>
                /// <param name="stream">文件头数据流</param>
                /// <returns>是否成功</returns>
                public fastCSharp.net.returnValue<bool> create(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream stream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o2> _wait_ = fastCSharp.net.waitCall<tcpServer._o2>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.create(stream, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o2> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void create(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream stream, Action<fastCSharp.net.returnValue<tcpServer._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o2> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                stream = stream,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c2, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 数据库文件数据加载
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                public void loadHeader(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer, tcpServer._o3>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        loadHeader(identity, null, _onOutput_, false);
                    }
                }


                private void loadHeader(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                identity = identity,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 数据库文件数据加载
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                public void load(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.code.cSharp.tcpBase.subByteArrayBuffer, tcpServer._o4>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        load(identity, null, _onOutput_, false);
                    }
                }


                private void load(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                identity = identity,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c5 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 5 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 数据库文件加载完毕
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                /// <param name="isLoaded">是否加载成功</param>
                /// <returns>是否加载成功</returns>
                public fastCSharp.net.returnValue<bool> loaded(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, bool isLoaded)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o5> _wait_ = fastCSharp.net.waitCall<tcpServer._o5>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.loaded(identity, isLoaded, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o5> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void loaded(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, bool isLoaded, Action<fastCSharp.net.returnValue<tcpServer._o5>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o5>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o5> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o5>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i5 _inputParameter_ = new tcpServer._i5
                            {
                                identity = identity,
                                isLoaded = isLoaded,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c5, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c6 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 6 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写入日志
                /// </summary>
                /// <param name="dataStream">日志数据</param>
                /// <returns>是否成功写入缓冲区</returns>
                public fastCSharp.net.returnValue<int> append(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o6> _wait_ = fastCSharp.net.waitCall<tcpServer._o6>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.append(dataStream, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o6> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }
                /// <summary>
                /// 写入日志
                /// </summary>
                /// <param name="dataStream">日志数据</param>
                /// <param name="_onReturn_">是否成功写入缓冲区</param>
                public void append(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream, Action<fastCSharp.net.returnValue<int>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<int, tcpServer._o6>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        append(dataStream, null, _onOutput_, false);
                    }
                }


                private void append(fastCSharp.code.cSharp.tcpBase.subByteUnmanagedStream dataStream, Action<fastCSharp.net.returnValue<tcpServer._o6>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o6> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o6>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i6 _inputParameter_ = new tcpServer._i6
                            {
                                dataStream = dataStream,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c6, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c7 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 7 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 等待缓存写入
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                public fastCSharp.net.returnValue waitBuffer(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.waitBuffer(identity, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 等待缓存写入
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void waitBuffer(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    waitBuffer(identity, _onReturn_, null, true);
                }
                private void waitBuffer(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i7 _inputParameter_ = new tcpServer._i7
                            {
                                identity = identity,
                            };
                            _socket_.Call<tcpServer._i7>(_onReturn_, _callback_, _c7, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c8 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 8 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写入缓存
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                /// <returns>是否成功</returns>
                public fastCSharp.net.returnValue<bool> flush(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o8> _wait_ = fastCSharp.net.waitCall<tcpServer._o8>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.flush(identity, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o8> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void flush(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, Action<fastCSharp.net.returnValue<tcpServer._o8>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o8>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o8> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o8>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i8 _inputParameter_ = new tcpServer._i8
                            {
                                identity = identity,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c8, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c9 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 9 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 写入缓存
                /// </summary>
                /// <param name="identity">数据库物理层唯一标识</param>
                /// <param name="isDiskFile">是否写入到磁盘文件</param>
                /// <returns>是否成功</returns>
                public fastCSharp.net.returnValue<bool> flushFile(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, bool isDiskFile)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o9> _wait_ = fastCSharp.net.waitCall<tcpServer._o9>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.flushFile(identity, isDiskFile, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o9> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void flushFile(fastCSharp.memoryDatabase.physicalServer.timeIdentity identity, bool isDiskFile, Action<fastCSharp.net.returnValue<tcpServer._o9>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o9>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o9> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o9>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i9 _inputParameter_ = new tcpServer._i9
                            {
                                identity = identity,
                                isDiskFile = isDiskFile,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c9, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c10 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 10 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o10> _wait_ = fastCSharp.net.waitCall<tcpServer._o10>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o10> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            ticks = _outputParameter_.Value.ticks;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<tcpServer._o10>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o10>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o10> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o10>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i10 _inputParameter_ = new tcpServer._i10
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c10, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
}namespace fastCSharp.net.tcp.http
{
        public partial class servers : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {

            /// <summary>
            /// httpServer TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.net.tcp.http.servers _value_;
                /// <summary>
                /// httpServer TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.net.tcp.http.servers value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("httpServer", typeof(fastCSharp.net.tcp.http.servers)))
                {
                    _value_ = value ?? new fastCSharp.net.tcp.http.servers();
                    setCommands(7);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[3 + 128].Set(3 + 128);
                    identityOnCommands[4 + 128].Set(4 + 128);
                    identityOnCommands[5 + 128].Set(5 + 128, 0);
                    identityOnCommands[verifyCommandIdentity = 6 + 128].Set(6 + 128, 1024);
                    _value_.SetCommandServer(this);
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
                            case 5: _M5(socket, ref data); return;
                            case 6: _M6(socket, ref data); return;
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public string assemblyPath;
                    public string serverType;
                    public fastCSharp.net.tcp.http.domain domain;
                    public bool isShareAssembly;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.net.tcp.http.servers.startState>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.net.tcp.http.servers.startState Ret;
                    public fastCSharp.net.tcp.http.servers.startState Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.net.tcp.http.servers.startState)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.net.tcp.http.servers, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            fastCSharp.net.tcp.http.servers.startState Return;


                            
                            Return = serverValue.start(inputParameter.assemblyPath, inputParameter.serverType, inputParameter.domain, inputParameter.isShareAssembly);

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
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i1
                {
                    public string assemblyPath;
                    public string serverType;
                    public fastCSharp.net.tcp.http.domain[] domains;
                    public bool isShareAssembly;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.net.tcp.http.servers.startState>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.net.tcp.http.servers.startState Ret;
                    public fastCSharp.net.tcp.http.servers.startState Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.net.tcp.http.servers.startState)value; }
                    }
#endif
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.net.tcp.http.servers, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            fastCSharp.net.tcp.http.servers.startState Return;


                            
                            Return = serverValue.start(inputParameter.assemblyPath, inputParameter.serverType, inputParameter.domains, inputParameter.isShareAssembly);

                            value.Value = new _o1
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
                        fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
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
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i2
                {
                    public fastCSharp.net.tcp.http.domain domain;
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.net.tcp.http.servers, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.stop(inputParameter.domain);

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
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i3
                {
                    public fastCSharp.net.tcp.http.domain[] domains;
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.net.tcp.http.servers, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.stop(inputParameter.domains);

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
                                fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i4
                {
                    public fastCSharp.net.tcp.host host;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.net.tcp.http.servers, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.setForward(inputParameter.host);

                            value.Value = new _o4
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
                        fastCSharp.net.returnValue<_o4> value = new fastCSharp.net.returnValue<_o4>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s4>.PushNotNull(this);
                    }
                }
                private void _M4(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i4 inputParameter = new _i4();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s4/**/.Call(socket, _value_, ref inputParameter );
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
                sealed class _s5 : fastCSharp.net.tcp.commandServer.serverCall<_s5, fastCSharp.net.tcp.http.servers>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.removeForward();

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
                        fastCSharp.typePool<_s5>.PushNotNull(this);
                    }
                }
                private void _M5(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            {
                                _s5/**/.Call(socket, _value_ );
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
                internal struct _i6
                {
                    public ulong randomPrefix;
                    public byte[] md5Data;
                    public long ticks;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public long ticks;
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
                sealed class _s6 : fastCSharp.net.tcp.commandServer.serverCall<_s6, fastCSharp.net.tcp.http.servers, _i6>
                {
                    private void get(ref fastCSharp.net.returnValue<_o6> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o6
                            {
                                ticks = inputParameter.ticks,
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
                        fastCSharp.net.returnValue<_o6> value = new fastCSharp.net.returnValue<_o6>();
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        fastCSharp.typePool<_s6>.PushNotNull(this);
                    }
                }
                private void _M6(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i6 inputParameter = new _i6();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s6/**/.Call(socket, _value_, ref inputParameter );
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
            public class tcpClient : fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("httpServer", typeof(fastCSharp.net.tcp.http.servers)), 28, verifyMethod ?? new fastCSharp.net.tcp.http.servers.tcpClient.timeVerifyMethod(), this);
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

                /// <summary>
                /// 启动域名服务
                /// </summary>
                /// <param name="assemblyPath">程序集文件名,包含路径</param>
                /// <param name="serverType">服务程序类型名称</param>
                /// <param name="domain">域名信息</param>
                /// <param name="isShareAssembly">是否共享程序集</param>
                /// <returns>域名服务启动状态</returns>
                public fastCSharp.net.returnValue<fastCSharp.net.tcp.http.servers.startState> start(string assemblyPath, string serverType, fastCSharp.net.tcp.http.domain domain, bool isShareAssembly)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.start(assemblyPath, serverType, domain, isShareAssembly, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<fastCSharp.net.tcp.http.servers.startState>{ Type = _returnType_ };
                }


                private void start(string assemblyPath, string serverType, fastCSharp.net.tcp.http.domain domain, bool isShareAssembly, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                assemblyPath = assemblyPath,
                                serverType = serverType,
                                domain = domain,
                                isShareAssembly = isShareAssembly,
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 启动域名服务
                /// </summary>
                /// <param name="assemblyPath">程序集文件名,包含路径</param>
                /// <param name="serverType">服务程序类型名称</param>
                /// <param name="domains">域名信息集合</param>
                /// <param name="isShareAssembly">是否共享程序集</param>
                /// <returns>域名服务启动状态</returns>
                public fastCSharp.net.returnValue<fastCSharp.net.tcp.http.servers.startState> start(string assemblyPath, string serverType, fastCSharp.net.tcp.http.domain[] domains, bool isShareAssembly)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o1> _wait_ = fastCSharp.net.waitCall<tcpServer._o1>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.start(assemblyPath, serverType, domains, isShareAssembly, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<fastCSharp.net.tcp.http.servers.startState>{ Type = _returnType_ };
                }


                private void start(string assemblyPath, string serverType, fastCSharp.net.tcp.http.domain[] domains, bool isShareAssembly, Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                assemblyPath = assemblyPath,
                                serverType = serverType,
                                domains = domains,
                                isShareAssembly = isShareAssembly,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 停止域名服务
                /// </summary>
                /// <param name="domain">域名信息</param>
                public fastCSharp.net.returnValue stop(fastCSharp.net.tcp.http.domain domain)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.stop(domain, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 停止域名服务
                /// </summary>
                /// <param name="domain">域名信息</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void stop(fastCSharp.net.tcp.http.domain domain, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    stop(domain, _onReturn_, null, true);
                }
                private void stop(fastCSharp.net.tcp.http.domain domain, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                domain = domain,
                            };
                            _socket_.Call<tcpServer._i2>(_onReturn_, _callback_, _c2, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 停止域名服务
                /// </summary>
                /// <param name="domains">域名信息集合</param>
                public fastCSharp.net.returnValue stop(fastCSharp.net.tcp.http.domain[] domains)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.stop(domains, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 停止域名服务
                /// </summary>
                /// <param name="domains">域名信息集合</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void stop(fastCSharp.net.tcp.http.domain[] domains, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    stop(domains, _onReturn_, null, true);
                }
                private void stop(fastCSharp.net.tcp.http.domain[] domains, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                domains = domains,
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 设置HTTP转发代理服务信息
                /// </summary>
                /// <param name="host">HTTP转发代理服务信息</param>
                /// <returns>是否设置成功</returns>
                public fastCSharp.net.returnValue<bool> setForward(fastCSharp.net.tcp.host host)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o4> _wait_ = fastCSharp.net.waitCall<tcpServer._o4>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.setForward(host, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void setForward(fastCSharp.net.tcp.host host, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                host = host,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c5 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 5 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 清除HTTP转发代理服务信息
                /// </summary>
                public fastCSharp.net.returnValue removeForward()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.removeForward(null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 清除HTTP转发代理服务信息
                /// </summary>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void removeForward(Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    removeForward(_onReturn_, null, true);
                }
                private void removeForward(Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Call(_onReturn_, _callback_, _c5, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c6 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 6 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o6> _wait_ = fastCSharp.net.waitCall<tcpServer._o6>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o6> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            ticks = _outputParameter_.Value.ticks;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<tcpServer._o6>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o6> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o6>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i6 _inputParameter_ = new tcpServer._i6
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c6, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
}namespace fastCSharp.net.tcp.http
{
        public partial class session<valueType> : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {

            /// <summary>
            /// session TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.net.tcp.http.session<valueType> _value_;
                /// <summary>
                /// session TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.net.tcp.http.session<valueType> value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("session", typeof(fastCSharp.net.tcp.http.session<valueType>)))
                {
                    _value_ = value ?? new fastCSharp.net.tcp.http.session<valueType>();
                    setCommands(4);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[verifyCommandIdentity = 3 + 128].Set(3 + 128, 1024);
                    _value_.SetCommandServer(this);
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
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public fastCSharp.sessionId sessionId;
                    public valueType value;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.sessionId>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.sessionId Ret;
                    public fastCSharp.sessionId Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.sessionId)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.net.tcp.http.session<valueType>, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            fastCSharp.sessionId Return;


                            
                            Return = serverValue.Set(inputParameter.sessionId, inputParameter.value);

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
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i1
                {
                    public fastCSharp.sessionId sessionId;
                    public valueType value;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public valueType value;
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
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.net.tcp.http.session<valueType>, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.TryGet(inputParameter.sessionId, out inputParameter.value);

                            value.Value = new _o1
                            {
                                value = inputParameter.value,
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
                        fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
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
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s1/**/.Call(socket, _value_, ref inputParameter );
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
                internal struct _i2
                {
                    public fastCSharp.sessionId sessionId;
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.net.tcp.http.session<valueType>, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.Remove(inputParameter.sessionId);

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
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _s2/**/.Call(socket, _value_, ref inputParameter );
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
                internal struct _i3
                {
                    public ulong randomPrefix;
                    public byte[] md5Data;
                    public long ticks;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public long ticks;
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
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.net.tcp.http.session<valueType>, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o3
                            {
                                ticks = inputParameter.ticks,
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
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
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
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("session", typeof(fastCSharp.net.tcp.http.session<valueType>)), 28, verifyMethod ?? new fastCSharp.net.tcp.http.session<valueType>.tcpClient.timeVerifyMethod(), this);
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

                public fastCSharp.net.returnValue<fastCSharp.sessionId> Set(fastCSharp.sessionId sessionId, valueType value)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Set(sessionId, value, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<fastCSharp.sessionId>{ Type = _returnType_ };
                }


                private void Set(fastCSharp.sessionId sessionId, valueType value, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                sessionId = sessionId,
                                value = value,
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> TryGet(fastCSharp.sessionId sessionId, out valueType value)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o1> _wait_ = fastCSharp.net.waitCall<tcpServer._o1>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.TryGet(sessionId, out value, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            value = _outputParameter_.Value.value;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    value = default(valueType);
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void TryGet(fastCSharp.sessionId sessionId, out valueType value, Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    value = default(valueType);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                sessionId = sessionId,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue Remove(fastCSharp.sessionId sessionId)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.Remove(sessionId, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void Remove(fastCSharp.sessionId sessionId, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    Remove(sessionId, _onReturn_, null, true);
                }
                private void Remove(fastCSharp.sessionId sessionId, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                sessionId = sessionId,
                            };
                            _socket_.Call<tcpServer._i2>(_onReturn_, _callback_, _c2, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o3> _wait_ = fastCSharp.net.waitCall<tcpServer._o3>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            ticks = _outputParameter_.Value.ticks;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
}namespace fastCSharp.net.tcp
{
        public partial class tcpRegister : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {

            /// <summary>
            /// tcpRegister TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.net.tcp.tcpRegister _value_;
                /// <summary>
                /// tcpRegister TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.net.tcp.tcpRegister value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("tcpRegister", typeof(fastCSharp.net.tcp.tcpRegister)))
                {
                    _value_ = value ?? new fastCSharp.net.tcp.tcpRegister();
                    setCommands(4);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[verifyCommandIdentity = 3 + 128].Set(3 + 128, 1024);
                    _value_.SetCommandServer(this);
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
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public fastCSharp.net.tcp.tcpRegisterReader.clientId clientId;
                    public fastCSharp.net.tcp.tcpRegisterReader.service service;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.net.tcp.tcpRegister.registerResult>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.net.tcp.tcpRegister.registerResult Ret;
                    public fastCSharp.net.tcp.tcpRegister.registerResult Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.net.tcp.tcpRegister.registerResult)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.net.tcp.tcpRegister, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            fastCSharp.net.tcp.tcpRegister.registerResult Return;


                            
                            Return = serverValue.register(inputParameter.clientId, inputParameter.service);

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
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i1
                {
                    public fastCSharp.net.tcp.tcpRegisterReader.clientId clientId;
                    public string serviceName;
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.net.tcp.tcpRegister, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.removeRegister(inputParameter.clientId, inputParameter.serviceName);

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
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i2
                {
                    public fastCSharp.net.tcp.tcpRegisterReader.clientId clientId;
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.net.tcp.tcpRegister, _i2>
                {
                    private void get(ref fastCSharp.net.returnValue value)
                    {
                        try
                        {
                            


                            serverValue.removeRegister(inputParameter.clientId);

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
                        fastCSharp.typePool<_s2>.PushNotNull(this);
                    }
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i3
                {
                    public ulong randomPrefix;
                    public byte[] md5Data;
                    public long ticks;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public long ticks;
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
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.net.tcp.tcpRegister, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o3
                            {
                                ticks = inputParameter.ticks,
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
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
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
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("tcpRegister", typeof(fastCSharp.net.tcp.tcpRegister)), 28, verifyMethod ?? new fastCSharp.net.tcp.tcpRegister.tcpClient.timeVerifyMethod(), this);
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

                /// <summary>
                /// 注册TCP服务信息
                /// </summary>
                /// <param name="clientId">TCP服务端标识</param>
                /// <param name="service">TCP服务信息</param>
                /// <returns>注册状态</returns>
                public fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegister.registerResult> register(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, fastCSharp.net.tcp.tcpRegisterReader.service service)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.register(clientId, service, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegister.registerResult>{ Type = _returnType_ };
                }


                private void register(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, fastCSharp.net.tcp.tcpRegisterReader.service service, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                clientId = clientId,
                                service = service,
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 注销TCP服务信息
                /// </summary>
                /// <param name="clientId">TCP服务端标识</param>
                /// <param name="serviceName">TCP服务名称</param>
                public fastCSharp.net.returnValue removeRegister(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, string serviceName)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.removeRegister(clientId, serviceName, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 注销TCP服务信息
                /// </summary>
                /// <param name="clientId">TCP服务端标识</param>
                /// <param name="serviceName">TCP服务名称</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void removeRegister(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, string serviceName, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    removeRegister(clientId, serviceName, _onReturn_, null, true);
                }
                private void removeRegister(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, string serviceName, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                clientId = clientId,
                                serviceName = serviceName,
                            };
                            _socket_.Call<tcpServer._i1>(_onReturn_, _callback_, _c1, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 注销TCP服务信息
                /// </summary>
                /// <param name="clientId">TCP服务端标识</param>
                public fastCSharp.net.returnValue removeRegister(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall _wait_ = fastCSharp.net.waitCall.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.removeRegister(clientId, null, _wait_, false);
                        fastCSharp.net.returnValue _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return _returnValue_;
                    }
                    return new fastCSharp.net.returnValue{ Type = _returnType_ };
                }


                /// <summary>
                /// 注销TCP服务信息
                /// </summary>
                /// <param name="clientId">TCP服务端标识</param>
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void removeRegister(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, Action<fastCSharp.net.returnValue> _onReturn_)
                {
                    
                    removeRegister(clientId, _onReturn_, null, true);
                }
                private void removeRegister(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                clientId = clientId,
                            };
                            _socket_.Call<tcpServer._i2>(_onReturn_, _callback_, _c2, ref _inputParameter_, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o3> _wait_ = fastCSharp.net.waitCall<tcpServer._o3>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            ticks = _outputParameter_.Value.ticks;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
}namespace fastCSharp.net.tcp
{
        public partial class tcpRegisterReader : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {

            /// <summary>
            /// tcpRegisterReader TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.net.tcp.tcpRegisterReader _value_;
                /// <summary>
                /// tcpRegisterReader TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.net.tcp.tcpRegisterReader value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("tcpRegisterReader", typeof(fastCSharp.net.tcp.tcpRegisterReader)))
                {
                    _value_ = value ?? new fastCSharp.net.tcp.tcpRegisterReader();
                    setCommands(4);
                    identityOnCommands[0 + 128].Set(0 + 128, 0);
                    identityOnCommands[1 + 128].Set(1 + 128);
                    identityOnCommands[2 + 128].Set(2 + 128);
                    identityOnCommands[verifyCommandIdentity = 3 + 128].Set(3 + 128, 1024);
                    _value_.SetCommandServer(this);
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
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.net.tcp.tcpRegisterReader.clientId>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.net.tcp.tcpRegisterReader.clientId Ret;
                    public fastCSharp.net.tcp.tcpRegisterReader.clientId Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.net.tcp.tcpRegisterReader.clientId)value; }
                    }
#endif
                }
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.net.tcp.tcpRegisterReader>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            fastCSharp.net.tcp.tcpRegisterReader.clientId Return;


                            
                            Return = serverValue.register();

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
                            {
                                _s0/**/.Call(socket, _value_ );
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
                internal struct _i1
                {
                    public int logIdentity;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.net.tcp.tcpRegisterReader.services[]>
#endif
                {
                    public int logIdentity;
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.net.tcp.tcpRegisterReader.services[] Ret;
                    public fastCSharp.net.tcp.tcpRegisterReader.services[] Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.net.tcp.tcpRegisterReader.services[])value; }
                    }
#endif
                }
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.net.tcp.tcpRegisterReader, _i1>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            fastCSharp.net.tcp.tcpRegisterReader.services[] Return;


                            
                            Return = serverValue.getServices(out inputParameter.logIdentity);

                            value.Value = new _o1
                            {
                                logIdentity = inputParameter.logIdentity,
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
                        fastCSharp.net.returnValue<_o1> value = new fastCSharp.net.returnValue<_o1>();
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
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket, _value_, ref inputParameter ));
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
                internal struct _i2
                {
                    public fastCSharp.net.tcp.tcpRegisterReader.clientId clientId;
                    public int logIdentity;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.net.tcp.tcpRegisterReader.log>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.net.tcp.tcpRegisterReader.log Ret;
                    public fastCSharp.net.tcp.tcpRegisterReader.log Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.net.tcp.tcpRegisterReader.log)value; }
                    }
#endif
                }
                private void _M2(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i2 inputParameter = new _i2();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _o2 outputParameter = new _o2();
                                Func<fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegisterReader.log>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o2, fastCSharp.net.tcp.tcpRegisterReader.log>.GetKeep(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.getLog(inputParameter.clientId, inputParameter.logIdentity, callbackReturn);
                                }
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
                internal struct _i3
                {
                    public ulong randomPrefix;
                    public byte[] md5Data;
                    public long ticks;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
#endif
                {
                    public long ticks;
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
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.net.tcp.tcpRegisterReader, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.verify(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                            if (Return) socket.SetVerifyMethod();
                            value.Value = new _o3
                            {
                                ticks = inputParameter.ticks,
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
                        fastCSharp.net.returnValue<_o3> value = new fastCSharp.net.returnValue<_o3>();
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
            }

            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, fastCSharp.net.tcp.commandClient.IClient
            {
                /// <summary>
                /// 时间验证服务客户端验证函数
                /// </summary>
#if NOJIT
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(object client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify((tcpClient)client);
                    }
                }
#else
                public sealed class timeVerifyMethod : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient>
                {
                    /// <summary>
                    /// 时间验证服务客户端验证函数
                    /// </summary>
                    /// <param name="client">TCP调用客户端</param>
                    /// <returns>是否通过验证</returns>
                    public bool Verify(tcpClient client)
                    {
                        return fastCSharp.net.tcp.timeVerifyServer.Verify(client);
                    }
                }
#endif
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("tcpRegisterReader", typeof(fastCSharp.net.tcp.tcpRegisterReader)), 28, verifyMethod ?? new fastCSharp.net.tcp.tcpRegisterReader.tcpClient.timeVerifyMethod(), this);
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

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// TCP服务端注册
                /// </summary>
                /// <returns>TCP服务端标识</returns>
                public fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegisterReader.clientId> register()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.register(null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegisterReader.clientId>{ Type = _returnType_ };
                }


                private void register(Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c0, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c1 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 获取TCP服务信息集合
                /// </summary>
                /// <param name="logIdentity">日志流编号</param>
                /// <returns>TCP服务信息集合</returns>
                public fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegisterReader.services[]> getServices(out int logIdentity)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o1> _wait_ = fastCSharp.net.waitCall<tcpServer._o1>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.getServices(out logIdentity, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            logIdentity = _outputParameter_.Value.logIdentity;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    logIdentity = default(int);
                    return new fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegisterReader.services[]>{ Type = _returnType_ };
                }


                private void getServices(out int logIdentity, Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    logIdentity = default(int);
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                            };
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 2147483647, IsKeepCallback = 1, IsSendOnly = 0 };

                /// <summary>
                /// TCP服务端轮询
                /// </summary>
                /// <param name="clientId">TCP服务端标识</param>
                /// <param name="logIdentity">日志编号</param>
                /// <returns>保持异步回调</returns>
                public fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback getLog(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, int logIdentity, Action<fastCSharp.net.returnValue<fastCSharp.net.tcp.tcpRegisterReader.log>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.net.tcp.tcpRegisterReader.log, tcpServer._o2>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        
                        return getLog(clientId, logIdentity, null, _onOutput_, true);
                    }
                    return null;
                }


                private fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback getLog(fastCSharp.net.tcp.tcpRegisterReader.clientId clientId, int logIdentity, Action<fastCSharp.net.returnValue<tcpServer._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o2> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i2 _inputParameter_ = new tcpServer._i2
                            {
                                clientId = clientId,
                                logIdentity = logIdentity,
                            };
                            
                            return _socket_.Get(_onReturn_, _callback_, _c2, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o3> _wait_ = fastCSharp.net.waitCall<tcpServer._o3>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            ticks = _outputParameter_.Value.ticks;
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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