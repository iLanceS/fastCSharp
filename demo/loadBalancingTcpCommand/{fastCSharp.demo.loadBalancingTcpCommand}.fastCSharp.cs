//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.loadBalancingTcpCommand
{
        public partial class server
        {

            /// <summary>
            /// loadBalancingTest TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.demo.loadBalancingTcpCommand.server _value_;
                /// <summary>
                /// loadBalancingTest TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null, fastCSharp.demo.loadBalancingTcpCommand.server value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("loadBalancingTest", typeof(fastCSharp.demo.loadBalancingTcpCommand.server)), verify)
                {
                    _value_ = value ?? new fastCSharp.demo.loadBalancingTcpCommand.server();
                    setCommands(2);
                    identityOnCommands[0 + 128].Set(0 + 128);
                    identityOnCommands[1 + 128].Set(1 + 128);
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
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public int left;
                    public int right;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
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
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.demo.loadBalancingTcpCommand.server, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.add(inputParameter.left, inputParameter.right);

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
                    public int left;
                    public int right;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<int>
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
                private void _M1(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i1 inputParameter = new _i1();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _o1 outputParameter = new _o1();
                                Func<fastCSharp.net.returnValue<int>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o1, int>.Get(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.xor(inputParameter.left, inputParameter.right, callbackReturn);
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
            }
            /// <summary>
            /// TCP负载均衡服务
            /// </summary>
            public sealed class tcpLoadBalancing : fastCSharp.net.tcp.commandLoadBalancingServer<tcpClient>
            {
                /// <summary>
                /// TCP负载均衡服务
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="verify">TCP验证实例</param>
#if NOJIT
                public tcpLoadBalancing(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#else
                public tcpLoadBalancing(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null)
#endif
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("loadBalancingTestLoadBalancing", typeof(fastCSharp.demo.loadBalancingTcpCommand.server)), verify)
                {
                }
                protected override tcpClient _createClient_(fastCSharp.code.cSharp.tcpServer attribute)
                {
                    tcpClient client = new tcpClient(attribute, _verify_);
                    fastCSharp.net.tcp.commandClient commandClient = client.TcpCommandClient;
                    if (commandClient != null && commandClient.StreamSocket != null) return client;
                    fastCSharp.pub.Dispose(ref client);
                    return null;
                }
                protected override DateTime _loadBalancingCheckTime_(tcpClient client)
                {
                    fastCSharp.net.tcp.commandClient tcpClient = client.TcpCommandClient;
                    return tcpClient != null ? tcpClient.LoadBalancingCheckTime : DateTime.MinValue;
                }
                protected override fastCSharp.net.returnValue.type _loadBalancingCheck_(tcpClient client)
                {
                    fastCSharp.net.tcp.commandClient tcpClient = client.TcpCommandClient;
                    return tcpClient == null ? fastCSharp.net.returnValue.type.ClientDisposed : tcpClient.LoadBalancingCheck();
                }

                public fastCSharp.net.returnValue<int> add(int left, int right)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    _returnType_ = fastCSharp.net.returnValue.type.Unknown;
                    int _tryCount_ = 3;
                    do
                    {
                        clientIdentity _client_ = _getClient_();
                        if (_client_.Client == null)
                        {
                            _returnType_ = fastCSharp.net.returnValue.type.ClientDisposed;
                            break;
                        }
                        try
                        {
                            fastCSharp.net.returnValue<int> _return_ = _client_.Client.add(left, right);
                            _end_(ref _client_, _return_.Type);
                            if (_return_.Type == fastCSharp.net.returnValue.type.Success || _return_.Type == fastCSharp.net.returnValue.type.VersionExpired) return _return_;
                            System.Threading.Thread.Sleep(1);
                        }
                        catch (Exception _error_)
                        {
                            _end_(ref _client_, _returnType_ = fastCSharp.net.returnValue.type.ClientException);
                            fastCSharp.log.Error.Add(_error_, null, false);
                        }
                    }
                    while (--_tryCount_ > 0);
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }
                sealed class _l1 : fastCSharp.code.cSharp.tcpBase.loadBalancingCallback<int>
                {
                    private clientIdentity _client_;
                    private tcpLoadBalancing _loadBalancingServer_;
                    private int left;
                    private int right;
                    protected override void _call_()
                    {
                        fastCSharp.net.returnValue.type _returnType_;
                        try
                        {
                            _client_ = _loadBalancingServer_._getClient_();
                            if (_client_.Client != null)
                            {
                                _client_.Client.xor(left, right, _onReturnHandle_);
                                return;
                            }
                            _returnType_ = fastCSharp.net.returnValue.type.ClientDisposed;
                        }
                        catch (Exception error)
                        {
                            _loadBalancingServer_._end_(ref _client_, _returnType_ = fastCSharp.net.returnValue.type.ClientException);
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                        _push_(new fastCSharp.net.returnValue<int>{ Type = _returnType_ });
                    }
                    protected override void _push_(fastCSharp.net.returnValue.type isReturn)
                    {
                        _loadBalancingServer_._end_(ref _client_, isReturn);
                        _loadBalancingServer_ = null;
                        left = default(int);
                        right = default(int);
                        fastCSharp.typePool<_l1>.PushNotNull(this);
                    }
                    public static void _Call_(tcpLoadBalancing _loadBalancingServer_,
                        int left, int right, Action<fastCSharp.net.returnValue<int>> _onReturn_)
                    {
                        _l1 _callback_ = fastCSharp.typePool<_l1>.Pop();
                        if (_callback_ == null)
                        {
                            try
                            {
                                _callback_ = new _l1();
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                                _onReturn_(new fastCSharp.net.returnValue<int>{ Type = fastCSharp.net.returnValue.type.ClientException });
                                return;
                            }
                        }
                        _callback_.left = left;
                        _callback_.right = right;
                        _callback_._loadBalancingServer_ = _loadBalancingServer_;
                        _callback_._onReturn_ = _onReturn_;
                        _callback_._tryCount_ =3;
                        _callback_._call_();
                    }
                }
                public void xor(int left, int right, Action<fastCSharp.net.returnValue<int>> _onReturn_)
                {
                    _l1/**/._Call_(this, left, right, _onReturn_);
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
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("loadBalancingTest", typeof(fastCSharp.demo.loadBalancingTcpCommand.server)), 28, verify);
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

                public fastCSharp.net.returnValue<int> add(int left, int right)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.add(left, right, null, _wait_, false);
                        fastCSharp.net.returnValue<tcpServer._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            return _outputParameter_.Value.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    return new fastCSharp.net.returnValue<int>{ Type = _returnType_ };
                }


                private void add(int left, int right, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                left = left,
                                right = right,
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

                public void xor(int left, int right, Action<fastCSharp.net.returnValue<int>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<int, tcpServer._o1>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        xor(left, right, null, _onOutput_, false);
                    }
                }


                private void xor(int left, int right, Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i1 _inputParameter_ = new tcpServer._i1
                            {
                                left = left,
                                right = right,
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
            }

        }
}
#endif