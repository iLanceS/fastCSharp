//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.sqlTableCacheServer
{
        public partial class Class
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static fastCSharp.demo.sqlTableCacheServer.Class _M0(int id)
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Class/**/.Get(id);
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static int[] _M1()
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Class/**/.getIds();
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static int[] _M2(int id)
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Class/**/.GetStudentIds(id);
                }
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// 班级表格定义
            /// </summary>
            public partial class Class
            {
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 获取班级信息
                /// </summary>
                /// <param name="id">班级标识</param>
                /// <returns>班级</returns>
                public static fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Class> Get(int id)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0>.Get();
                    if (_wait_ != null)
                    {
                        
                        Get(id, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Class>{ Type = _returnType_ };
                }


                private static void Get(int id, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataReader/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i0 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i0
                            {
                                id = id,
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

                /// <summary>
                /// 获取班级标识集合
                /// </summary>
                /// <returns>班级标识集合</returns>
                public static fastCSharp.net.returnValue<int[]> getIds()
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1>.Get();
                    if (_wait_ != null)
                    {
                        
                        getIds(null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<int[]>{ Type = _returnType_ };
                }


                private static void getIds(Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataReader/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            _socket_.Get(_onReturn_, _callback_, _c1, ref _returnType_.Value, _isTask_);
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
                /// 获取学生标识集合
                /// </summary>
                /// <param name="id">班级标识</param>
                /// <returns>学生标识集合</returns>
                public static fastCSharp.net.returnValue<int[]> GetStudentIds(int id)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2>.Get();
                    if (_wait_ != null)
                    {
                        
                        GetStudentIds(id, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<int[]>{ Type = _returnType_ };
                }


                private static void GetStudentIds(int id, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataReader/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i2 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i2
                            {
                                id = id,
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
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        public partial class Student
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static fastCSharp.demo.sqlTableCacheServer.Student _M6(int id)
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Student/**/.Get(id);
                }
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// 学生表格定义
            /// </summary>
            public partial class Student
            {
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c6 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                /// <summary>
                /// 获取学生信息
                /// </summary>
                /// <param name="id">学生标识</param>
                /// <returns>学生</returns>
                public static fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Student> Get(int id)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3>.Get();
                    if (_wait_ != null)
                    {
                        
                        Get(id, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Student>{ Type = _returnType_ };
                }


                private static void Get(int id, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataReader/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i3 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i3
                            {
                                id = id,
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
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        internal partial class dataReaderTcpCallVerify
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static bool _M11(fastCSharp.net.tcp.commandServer.socket socket, ulong randomPrefix, byte[] md5Data, ref long ticks)
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.dataReaderTcpCallVerify/**/.verify(socket, randomPrefix, md5Data, ref ticks);
                }
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// TCP调用验证
            /// </summary>
            public partial class dataReaderTcpCallVerify : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod
            {
                /// <summary>
                /// TCP调用验证客户端
                /// </summary>
                /// <returns></returns>
                public bool Verify()
                {
                    return fastCSharp.net.tcp.timeVerifyServer.tcpCall<fastCSharp.demo.sqlTableCacheServer.dataReaderTcpCallVerify>.Verify(fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataReader/**/.Default, verify);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c11 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public static fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4>.Get();
                    if (_wait_ != null)
                    {
                        
                        verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4 _outputParameterValue_ = _outputParameter_.Value;
                            ticks = _outputParameterValue_.ticks;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private static void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataReader/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i4 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataReader/**/._i4
                            {
                                randomPrefix = randomPrefix,
                                md5Data = md5Data,
                                ticks = ticks,
                            };
                            
                            _returnType_.Value.ticks = _inputParameter_.ticks;
                            _socket_.Get(_onReturn_, _callback_, _c11, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
namespace fastCSharp.demo.sqlTableCacheServer.tcpServer
{

        /// <summary>
        /// TCP调用服务端
        /// </summary>
        public partial class DataReader : fastCSharp.net.tcp.commandServer
        {
            /// <summary>
            /// TCP调用服务端
            /// </summary>
            /// <param name="attribute">TCP调用服务器端配置信息</param>
            /// <param name="verify">TCP验证实例</param>
            public DataReader(fastCSharp.code.cSharp.tcpServer attribute = null)
                : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("DataReader", typeof(fastCSharp.demo.sqlTableCacheServer.dataReaderTcpCallVerify)))
            {
                setCommands(5);
                identityOnCommands[0 + 128].Set(0 + 128);
                identityOnCommands[1 + 128].Set(1 + 128, 0);
                identityOnCommands[2 + 128].Set(2 + 128);
                identityOnCommands[3 + 128].Set(3 + 128);
                identityOnCommands[verifyCommandIdentity = 4 + 128].Set(4 + 128, 1024);
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
            /// <summary>
            /// 忽略分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            protected override void ignoreGroup(int groupId)
            {
            }

            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i0
            {
                public int id;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.sqlTableCacheServer.Class>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.demo.sqlTableCacheServer.Class Ret;
                public fastCSharp.demo.sqlTableCacheServer.Class Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.demo.sqlTableCacheServer.Class)value; }
                }
#endif
            }
            sealed class _s0 : fastCSharp.net.tcp.commandServer.socketCall<_s0, _i0>
            {
                private void get(ref fastCSharp.net.returnValue<_o0> value)
                {
                    try
                    {
                        
                        fastCSharp.demo.sqlTableCacheServer.Class Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Class/**/.tcpServer/**/._M0(inputParameter.id);

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

                            fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket, ref inputParameter));
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<int[]>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public int[] Ret;
                public int[] Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (int[])value; }
                }
#endif
            }
            sealed class _s1 : fastCSharp.net.tcp.commandServer.socketCall<_s1>
            {
                private void get(ref fastCSharp.net.returnValue<_o1> value)
                {
                    try
                    {
                        
                        int[] Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Class/**/.tcpServer/**/._M1();

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
                        {

                            fastCSharp.threading.task.Tiny.Add(_s1/**/.GetCall(socket));
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i2
            {
                public int id;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<int[]>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public int[] Ret;
                public int[] Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (int[])value; }
                }
#endif
            }
            sealed class _s2 : fastCSharp.net.tcp.commandServer.socketCall<_s2, _i2>
            {
                private void get(ref fastCSharp.net.returnValue<_o2> value)
                {
                    try
                    {
                        
                        int[] Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Class/**/.tcpServer/**/._M2(inputParameter.id);

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

                            fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket, ref inputParameter));
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i3
            {
                public int id;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.sqlTableCacheServer.Student>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.demo.sqlTableCacheServer.Student Ret;
                public fastCSharp.demo.sqlTableCacheServer.Student Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.demo.sqlTableCacheServer.Student)value; }
                }
#endif
            }
            sealed class _s3 : fastCSharp.net.tcp.commandServer.socketCall<_s3, _i3>
            {
                private void get(ref fastCSharp.net.returnValue<_o3> value)
                {
                    try
                    {
                        
                        fastCSharp.demo.sqlTableCacheServer.Student Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Student/**/.tcpServer/**/._M6(inputParameter.id);

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

                            fastCSharp.threading.task.Tiny.Add(_s3/**/.GetCall(socket, ref inputParameter));
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
                public ulong randomPrefix;
                public byte[] md5Data;
                public long ticks;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
            sealed class _s4 : fastCSharp.net.tcp.commandServer.socketCall<_s4, _i4>
            {
                private void get(ref fastCSharp.net.returnValue<_o4> value)
                {
                    try
                    {
                        
                        bool Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.dataReaderTcpCallVerify/**/.tcpServer/**/._M11(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                        if (Return) socket.SetVerifyMethod();
                        value.Value = new _o4
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

                            _s4/**/.Call(socket, ref inputParameter);
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
}
namespace fastCSharp.demo.sqlTableCacheServer.tcpClient
{

        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public class DataReader
        {
            /// <summary>
            /// 默认TCP调用服务器端配置信息
            /// </summary>
            protected internal static readonly fastCSharp.code.cSharp.tcpServer defaultTcpServer;
            /// <summary>
            /// 默认客户端TCP调用
            /// </summary>
            public static readonly fastCSharp.net.tcp.commandClient Default;
            static DataReader()
            {
                defaultTcpServer = fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("DataReader", typeof(fastCSharp.demo.sqlTableCacheServer.dataReaderTcpCallVerify));
                if (defaultTcpServer.IsServer) fastCSharp.log.Error.Add("请确认 DataReader 服务器端是否本地调用", null, false);
                Default = new fastCSharp.net.tcp.commandClient(defaultTcpServer, 28, new fastCSharp.demo.sqlTableCacheServer.tcpCall.dataReaderTcpCallVerify());
                if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(typeof(DataReader));
            }
            /// <summary>
            /// 忽略TCP分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            /// <returns>是否调用成功</returns>
            public static fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
            {
                fastCSharp.net.tcp.commandClient client = Default;
                return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        public partial class Class
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity _M3()
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Class/**/.getSqlCache();
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static void _M4(long ticks, int identity, System.Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data>,bool> onLog)
                {

                    fastCSharp.demo.sqlTableCacheServer.Class/**/.onSqlLog(ticks, identity, onLog);
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static fastCSharp.demo.sqlTableCacheServer.Class _M5(int Id)
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Class/**/.getSqlCache(Id);
                }
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// 班级表格定义
            /// </summary>
            public partial class Class
            {
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c3 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                public static void getSqlCache(Action<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o0>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity, fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o0>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        getSqlCache(null, _onOutput_, false);
                    }
                }


                private static void getSqlCache(Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o0> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            _socket_.Get(_onReturn_, _callback_, _c3, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c4 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 1 + 128, MaxInputSize = 2147483647, IsKeepCallback = 1, IsSendOnly = 0 };

                /// <returns>保持异步回调</returns>
                public static fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback onSqlLog(long ticks, int identity, Action<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o1>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data, fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o1>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        
                        return onSqlLog(ticks, identity, null, _onOutput_, false);
                    }
                    return null;
                }


                private static fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback onSqlLog(long ticks, int identity, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o1> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i1 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i1
                            {
                                ticks = ticks,
                                identity = identity,
                            };
                            
                            
                            return _socket_.Get(_onReturn_, _callback_, _c4, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c5 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 5 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public static fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Class> getSqlCache(int Id)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5>.Get();
                    if (_wait_ != null)
                    {
                        
                        getSqlCache(Id, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Class>{ Type = _returnType_ };
                }


                private static void getSqlCache(int Id, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o5>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i5 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i5
                            {
                                Id = Id,
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
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        public partial class Student
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity _M7()
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Student/**/.getSqlCache();
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static void _M8(long ticks, int identity, System.Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data>,bool> onLog)
                {

                    fastCSharp.demo.sqlTableCacheServer.Student/**/.onSqlLog(ticks, identity, onLog);
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static fastCSharp.demo.sqlTableCacheServer.Student _M9(int Id)
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.Student/**/.getSqlCache(Id);
                }
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// 学生表格定义
            /// </summary>
            public partial class Student
            {
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c7 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                public static void getSqlCache(Action<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o2>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity, fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o2>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        getSqlCache(null, _onOutput_, false);
                    }
                }


                private static void getSqlCache(Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o2> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            _socket_.Get(_onReturn_, _callback_, _c7, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c8 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 3 + 128, MaxInputSize = 2147483647, IsKeepCallback = 1, IsSendOnly = 0 };

                /// <returns>保持异步回调</returns>
                public static fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback onSqlLog(long ticks, int identity, Action<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o3>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data, fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o3>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        
                        return onSqlLog(ticks, identity, null, _onOutput_, false);
                    }
                    return null;
                }


                private static fastCSharp.net.tcp.commandClient.streamCommandSocket.keepCallback onSqlLog(long ticks, int identity, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o3> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i3 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i3
                            {
                                ticks = ticks,
                                identity = identity,
                            };
                            
                            
                            return _socket_.Get(_onReturn_, _callback_, _c8, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c9 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 6 + 128, MaxInputSize = 2147483647, IsKeepCallback = 0, IsSendOnly = 0 };

                public static fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Student> getSqlCache(int Id)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6>.Get();
                    if (_wait_ != null)
                    {
                        
                        getSqlCache(Id, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6 _outputParameterValue_ = _outputParameter_.Value;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.Student>{ Type = _returnType_ };
                }


                private static void getSqlCache(int Id, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o6>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.StreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i6 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i6
                            {
                                Id = Id,
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
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        internal partial class dataLogTcpCallVerify
        {
            internal static partial class tcpServer
            {
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static bool _M10(fastCSharp.net.tcp.commandServer.socket socket, ulong randomPrefix, byte[] md5Data, ref long ticks)
                {

                    
                    return fastCSharp.demo.sqlTableCacheServer.dataLogTcpCallVerify/**/.verify(socket, randomPrefix, md5Data, ref ticks);
                }
            }
        }
}namespace fastCSharp.demo.sqlTableCacheServer
{
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            /// <summary>
            /// TCP调用验证
            /// </summary>
            public partial class dataLogTcpCallVerify : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod
            {
                /// <summary>
                /// TCP调用验证客户端
                /// </summary>
                /// <returns></returns>
                public bool Verify()
                {
                    return fastCSharp.net.tcp.timeVerifyServer.tcpCall<fastCSharp.demo.sqlTableCacheServer.dataLogTcpCallVerify>.Verify(fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default, verify);
                }
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c10 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 4 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public static fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4> _wait_ = fastCSharp.net.waitCall<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4>.Get();
                    if (_wait_ != null)
                    {
                        
                        verify(randomPrefix, md5Data, ref ticks, null, _wait_, false);
                        fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4> _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {

                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4 _outputParameterValue_ = _outputParameter_.Value;
                            ticks = _outputParameterValue_.ticks;
                            return _outputParameterValue_.Return;
                        }
                        _returnType_ = _outputParameter_.Type;
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    return new fastCSharp.net.returnValue<bool>{ Type = _returnType_ };
                }


                private static void verify(ulong randomPrefix, byte[] md5Data, ref long ticks, Action<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4> _returnType_ = new fastCSharp.net.returnValue<fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = fastCSharp.demo.sqlTableCacheServer.tcpClient/**/.DataLog/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = _client_.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            
                            fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i4 _inputParameter_ = new fastCSharp.demo.sqlTableCacheServer.tcpServer/**/.DataLog/**/._i4
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
}
namespace fastCSharp.demo.sqlTableCacheServer.tcpServer
{

        /// <summary>
        /// TCP调用服务端
        /// </summary>
        public partial class DataLog : fastCSharp.net.tcp.commandServer
        {
            /// <summary>
            /// TCP调用服务端
            /// </summary>
            /// <param name="attribute">TCP调用服务器端配置信息</param>
            /// <param name="verify">TCP验证实例</param>
            public DataLog(fastCSharp.code.cSharp.tcpServer attribute = null)
                : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("DataLog", typeof(fastCSharp.demo.sqlTableCacheServer.dataLogTcpCallVerify)))
            {
                setCommands(7);
                identityOnCommands[0 + 128].Set(0 + 128, 0);
                identityOnCommands[1 + 128].Set(1 + 128);
                identityOnCommands[2 + 128].Set(2 + 128, 0);
                identityOnCommands[3 + 128].Set(3 + 128);
                identityOnCommands[verifyCommandIdentity = 4 + 128].Set(4 + 128, 1024);
                identityOnCommands[5 + 128].Set(5 + 128);
                identityOnCommands[6 + 128].Set(6 + 128);
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
            /// <summary>
            /// 忽略分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            protected override void ignoreGroup(int groupId)
            {
            }

            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o0 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity Ret;
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity)value; }
                }
#endif
            }
            sealed class _s0 : fastCSharp.net.tcp.commandServer.socketCall<_s0>
            {
                private void get(ref fastCSharp.net.returnValue<_o0> value)
                {
                    try
                    {
                        
                        fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.cacheIdentity Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Class/**/.tcpServer/**/._M3();

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

                            fastCSharp.threading.task.Tiny.Add(_s0/**/.GetCall(socket));
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i1
            {
                public long ticks;
                public int identity;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o1 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data Ret;
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data)value; }
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
                            Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o1, fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Class,fastCSharp.demo.sqlModel.Class>.data>.GetKeep(socket, ref outputParameter, 0);
                            if (callbackReturn != null)
                            {
                                fastCSharp.demo.sqlTableCacheServer.Class/**/.tcpServer/**/._M4(inputParameter.ticks, inputParameter.identity, callbackReturn);
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity Ret;
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity)value; }
                }
#endif
            }
            sealed class _s2 : fastCSharp.net.tcp.commandServer.socketCall<_s2>
            {
                private void get(ref fastCSharp.net.returnValue<_o2> value)
                {
                    try
                    {
                        
                        fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.cacheIdentity Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Student/**/.tcpServer/**/._M7();

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
                        {

                            fastCSharp.threading.task.Tiny.Add(_s2/**/.GetCall(socket));
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i3
            {
                public long ticks;
                public int identity;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data Ret;
                public fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data)value; }
                }
#endif
            }
            private void _M3(socket socket, ref subArray<byte> data)
            {
                fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                    try
                    {
                        _i3 inputParameter = new _i3();
                        if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                        {
                            _o3 outputParameter = new _o3();
                            Func<fastCSharp.net.returnValue<fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o3, fastCSharp.sql.logStream<fastCSharp.demo.sqlTableCacheServer.Student,fastCSharp.demo.sqlModel.Student>.data>.GetKeep(socket, ref outputParameter, 0);
                            if (callbackReturn != null)
                            {
                                fastCSharp.demo.sqlTableCacheServer.Student/**/.tcpServer/**/._M8(inputParameter.ticks, inputParameter.identity, callbackReturn);
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
            internal struct _i4
            {
                public ulong randomPrefix;
                public byte[] md5Data;
                public long ticks;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<bool>
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
            sealed class _s4 : fastCSharp.net.tcp.commandServer.socketCall<_s4, _i4>
            {
                private void get(ref fastCSharp.net.returnValue<_o4> value)
                {
                    try
                    {
                        
                        bool Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.dataLogTcpCallVerify/**/.tcpServer/**/._M10(socket, inputParameter.randomPrefix, inputParameter.md5Data, ref inputParameter.ticks);

                        if (Return) socket.SetVerifyMethod();
                        value.Value = new _o4
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

                            _s4/**/.Call(socket, ref inputParameter);
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i5
            {
                public int Id;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.sqlTableCacheServer.Class>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.demo.sqlTableCacheServer.Class Ret;
                public fastCSharp.demo.sqlTableCacheServer.Class Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.demo.sqlTableCacheServer.Class)value; }
                }
#endif
            }
            sealed class _s5 : fastCSharp.net.tcp.commandServer.socketCall<_s5, _i5>
            {
                private void get(ref fastCSharp.net.returnValue<_o5> value)
                {
                    try
                    {
                        
                        fastCSharp.demo.sqlTableCacheServer.Class Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Class/**/.tcpServer/**/._M5(inputParameter.Id);

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

                            _s5/**/.Call(socket, ref inputParameter);
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
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            internal struct _i6
            {
                public int Id;
            }
            [fastCSharp.emit.dataSerialize(IsMemberMap = false)]
            [fastCSharp.emit.boxSerialize]
#if NOJIT
            internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
            internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.sqlTableCacheServer.Student>
#endif
            {
                [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                public fastCSharp.demo.sqlTableCacheServer.Student Ret;
                public fastCSharp.demo.sqlTableCacheServer.Student Return
                {
                    get { return Ret; }
                    set { Ret = value; }
                }
#if NOJIT
                public object ReturnObject
                {
                    get { return Ret; }
                    set { Ret = (fastCSharp.demo.sqlTableCacheServer.Student)value; }
                }
#endif
            }
            sealed class _s6 : fastCSharp.net.tcp.commandServer.socketCall<_s6, _i6>
            {
                private void get(ref fastCSharp.net.returnValue<_o6> value)
                {
                    try
                    {
                        
                        fastCSharp.demo.sqlTableCacheServer.Student Return;


                        
                        Return = fastCSharp.demo.sqlTableCacheServer.Student/**/.tcpServer/**/._M9(inputParameter.Id);

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

                            _s6/**/.Call(socket, ref inputParameter);
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
}
namespace fastCSharp.demo.sqlTableCacheServer.tcpClient
{

        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public class DataLog
        {
            /// <summary>
            /// 默认TCP调用服务器端配置信息
            /// </summary>
            protected internal static readonly fastCSharp.code.cSharp.tcpServer defaultTcpServer;
            /// <summary>
            /// 默认客户端TCP调用
            /// </summary>
            public static readonly fastCSharp.net.tcp.commandClient Default;
            static DataLog()
            {
                defaultTcpServer = fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("DataLog", typeof(fastCSharp.demo.sqlTableCacheServer.dataLogTcpCallVerify));
                if (defaultTcpServer.IsServer) fastCSharp.log.Error.Add("请确认 DataLog 服务器端是否本地调用", null, false);
                Default = new fastCSharp.net.tcp.commandClient(defaultTcpServer, 28, new fastCSharp.demo.sqlTableCacheServer.tcpCall.dataLogTcpCallVerify());
                if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(typeof(DataLog));
            }
            /// <summary>
            /// 忽略TCP分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            /// <returns>是否调用成功</returns>
            public static fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId)
            {
                fastCSharp.net.tcp.commandClient client = Default;
                return client == null ? fastCSharp.net.returnValue.type.ClientDisposed : client.IgnoreGroup(groupId);
            }
        }
}
#endif