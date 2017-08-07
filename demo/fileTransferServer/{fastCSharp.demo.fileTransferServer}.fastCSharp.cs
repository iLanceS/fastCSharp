//本文件由程序自动生成,请不要自行修改
using System;
using fastCSharp;

#if NotFastCSharpCode
#else
#pragma warning disable
namespace fastCSharp.demo.fileTransferServer
{
        internal partial class permission
        {
            /// <summary>
            /// 关键字
            /// </summary>
            [fastCSharp.code.ignore]
            public struct primaryKey : IEquatable<primaryKey>
            {
                public string UserName;
                public string Path;
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="other">关键字</param>
                /// <returns>是否相等</returns>
                public bool Equals(primaryKey other)
                {
                    return UserName/**/.Equals(other.UserName) && Path/**/.Equals(other.Path);
                }
                /// <summary>
                /// 哈希编码
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return UserName.GetHashCode() ^ Path/**/.GetHashCode();
                }
                /// <summary>
                /// 关键字比较
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public override bool Equals(object obj)
                {
                    return Equals((primaryKey)obj);
                }
            }
        }
}namespace fastCSharp.demo.fileTransferServer
{
        public partial class server
        {

            /// <summary>
            /// fileTransfer TCP服务
            /// </summary>
            public sealed class tcpServer : fastCSharp.net.tcp.commandServer
            {
                private readonly fastCSharp.demo.fileTransferServer.server _value_;
                /// <summary>
                /// fileTransfer TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                /// <param name="value">TCP服务目标对象</param>
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.demo.fileTransferServer.server value = null)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("fileTransfer", typeof(fastCSharp.demo.fileTransferServer.server)))
                {
                    _value_ = value ?? new fastCSharp.demo.fileTransferServer.server();
                    setCommands(9);
                    identityOnCommands[verifyCommandIdentity = 0 + 128].Set(0 + 128, 1024);
                    identityOnCommands[1 + 128].Set(1 + 128, 0);
                    identityOnCommands[2 + 128].Set(2 + 128, 0);
                    identityOnCommands[3 + 128].Set(3 + 128);
                    identityOnCommands[4 + 128].Set(4 + 128);
                    identityOnCommands[5 + 128].Set(5 + 128);
                    identityOnCommands[6 + 128].Set(6 + 128);
                    identityOnCommands[7 + 128].Set(7 + 128);
                    identityOnCommands[8 + 128].Set(8 + 128);
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
                            default: return;
                        }
                    }
                }

                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                internal struct _i0
                {
                    public string userName;
                    public byte[] password;
                    public System.DateTime verifyTime;
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
                sealed class _s0 : fastCSharp.net.tcp.commandServer.serverCall<_s0, fastCSharp.demo.fileTransferServer.server, _i0>
                {
                    private void get(ref fastCSharp.net.returnValue<_o0> value)
                    {
                        try
                        {
                            
                            bool Return;


                            
                            Return = serverValue.login(socket, inputParameter.userName, inputParameter.password, inputParameter.verifyTime);

                            if (Return) socket.SetVerifyMethod();
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
                sealed class _s1 : fastCSharp.net.tcp.commandServer.serverCall<_s1, fastCSharp.demo.fileTransferServer.server>
                {
                    private void get(ref fastCSharp.net.returnValue<_o1> value)
                    {
                        try
                        {
                            
                            int Return;


                            
                            Return = serverValue.getBackupIdentity();

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
                internal struct _o2 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.fileTransferServer.server.pathPermission[]>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.fileTransferServer.server.pathPermission[] Ret;
                    public fastCSharp.demo.fileTransferServer.server.pathPermission[] Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.fileTransferServer.server.pathPermission[])value; }
                    }
#endif
                }
                sealed class _s2 : fastCSharp.net.tcp.commandServer.serverCall<_s2, fastCSharp.demo.fileTransferServer.server>
                {
                    private void get(ref fastCSharp.net.returnValue<_o2> value)
                    {
                        try
                        {
                            
                            fastCSharp.demo.fileTransferServer.server.pathPermission[] Return;


                            
                            Return = serverValue.getPermissions(socket);

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
                                _s2/**/.Call(socket, _value_ );
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
                    public string path;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o3 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.fileTransferServer.server.listName[]>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.fileTransferServer.server.listName[] Ret;
                    public fastCSharp.demo.fileTransferServer.server.listName[] Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.fileTransferServer.server.listName[])value; }
                    }
#endif
                }
                sealed class _s3 : fastCSharp.net.tcp.commandServer.serverCall<_s3, fastCSharp.demo.fileTransferServer.server, _i3>
                {
                    private void get(ref fastCSharp.net.returnValue<_o3> value)
                    {
                        try
                        {
                            
                            fastCSharp.demo.fileTransferServer.server.listName[] Return;


                            
                            Return = serverValue.list(socket, inputParameter.path);

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
                internal struct _i4
                {
                    public string path;
                    public fastCSharp.demo.fileTransferServer.server.listName[] listNames;
                    public int backupIdentity;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o4 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.fileTransferServer.server.listName[]>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.fileTransferServer.server.listName[] Ret;
                    public fastCSharp.demo.fileTransferServer.server.listName[] Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.fileTransferServer.server.listName[])value; }
                    }
#endif
                }
                sealed class _s4 : fastCSharp.net.tcp.commandServer.serverCall<_s4, fastCSharp.demo.fileTransferServer.server, _i4>
                {
                    private void get(ref fastCSharp.net.returnValue<_o4> value)
                    {
                        try
                        {
                            
                            fastCSharp.demo.fileTransferServer.server.listName[] Return;


                            
                            Return = serverValue.delete(socket, inputParameter.path, inputParameter.listNames, inputParameter.backupIdentity);

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
                    public fastCSharp.demo.fileTransferServer.server.listName listName;
                    public fastCSharp.code.cSharp.tcpBase.tcpStream fileStream;
                    public int backupIdentity;
                    public bool isTimeVersion;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o5 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.fileTransferServer.server.fileState>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.fileTransferServer.server.fileState Ret;
                    public fastCSharp.demo.fileTransferServer.server.fileState Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.fileTransferServer.server.fileState)value; }
                    }
#endif
                }
                private void _M5(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i5 inputParameter = new _i5();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _o5 outputParameter = new _o5();
                                Func<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.fileState>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o5, fastCSharp.demo.fileTransferServer.server.fileState>.Get(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.upload(socket, inputParameter.listName, socket.GetTcpStream(ref inputParameter.fileStream), inputParameter.backupIdentity, inputParameter.isTimeVersion, callbackReturn);
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
                internal struct _i6
                {
                    public fastCSharp.demo.fileTransferServer.server.listName listName;
                    public byte[] data;
                    public int backupIdentity;
                    public bool isTimeVersion;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o6 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.fileTransferServer.server.fileState>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.fileTransferServer.server.fileState Ret;
                    public fastCSharp.demo.fileTransferServer.server.fileState Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.fileTransferServer.server.fileState)value; }
                    }
#endif
                }
                sealed class _s6 : fastCSharp.net.tcp.commandServer.serverCall<_s6, fastCSharp.demo.fileTransferServer.server, _i6>
                {
                    private void get(ref fastCSharp.net.returnValue<_o6> value)
                    {
                        try
                        {
                            
                            fastCSharp.demo.fileTransferServer.server.fileState Return;


                            
                            Return = serverValue.upload(socket, inputParameter.listName, inputParameter.data, inputParameter.backupIdentity, inputParameter.isTimeVersion);

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
                                fastCSharp.threading.task.Tiny.Add(_s6/**/.GetCall(socket, _value_, ref inputParameter ));
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
                    public string path;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o7 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o7 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.fileTransferServer.server.fileState>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.fileTransferServer.server.fileState Ret;
                    public fastCSharp.demo.fileTransferServer.server.fileState Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.fileTransferServer.server.fileState)value; }
                    }
#endif
                }
                sealed class _s7 : fastCSharp.net.tcp.commandServer.serverCall<_s7, fastCSharp.demo.fileTransferServer.server, _i7>
                {
                    private void get(ref fastCSharp.net.returnValue<_o7> value)
                    {
                        try
                        {
                            
                            fastCSharp.demo.fileTransferServer.server.fileState Return;


                            
                            Return = serverValue.createDirectory(socket, inputParameter.path);

                            value.Value = new _o7
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
                        fastCSharp.net.returnValue<_o7> value = new fastCSharp.net.returnValue<_o7>();
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
                    public fastCSharp.demo.fileTransferServer.server.listName listName;
                    public fastCSharp.code.cSharp.tcpBase.tcpStream fileStream;
                }
                [fastCSharp.emit.dataSerialize(IsMemberMap = false, IsReferenceMember = false)]
                [fastCSharp.emit.boxSerialize]
#if NOJIT
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter
#else
                internal struct _o8 : fastCSharp.net.asynchronousMethod.IReturnParameter<fastCSharp.demo.fileTransferServer.server.listName>
#endif
                {
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public fastCSharp.demo.fileTransferServer.server.listName Ret;
                    public fastCSharp.demo.fileTransferServer.server.listName Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (fastCSharp.demo.fileTransferServer.server.listName)value; }
                    }
#endif
                }
                private void _M8(socket socket, ref subArray<byte> data)
                {
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                        try
                        {
                            _i8 inputParameter = new _i8();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            {
                                _o8 outputParameter = new _o8();
                                Func<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.listName>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<_o8, fastCSharp.demo.fileTransferServer.server.listName>.Get(socket, ref outputParameter, 0);
                                if (callbackReturn != null)
                                {
                                    
                                    _value_.download(socket, inputParameter.listName, socket.GetTcpStream(ref inputParameter.fileStream), callbackReturn);
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
                /// <param name="verifyMethod">TCP验证方法</param>
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null)
#endif
                {
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("fileTransfer", typeof(fastCSharp.demo.fileTransferServer.server)), 28, verifyMethod, this);
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

                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c0 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 0 + 128, MaxInputSize = 1024, IsKeepCallback = 0, IsSendOnly = 0 };

                public fastCSharp.net.returnValue<bool> login(string userName, byte[] password, System.DateTime verifyTime)
                {
                    fastCSharp.net.returnValue.type _returnType_;
                    fastCSharp.net.waitCall<tcpServer._o0> _wait_ = fastCSharp.net.waitCall<tcpServer._o0>.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        
                        this.login(userName, password, verifyTime, null, _wait_, false);
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


                private void login(string userName, byte[] password, System.DateTime verifyTime, Action<fastCSharp.net.returnValue<tcpServer._o0>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o0>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o0> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o0>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.VerifyStreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i0 _inputParameter_ = new tcpServer._i0
                            {
                                userName = userName,
                                password = password,
                                verifyTime = verifyTime,
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

                public void getBackupIdentity(Action<fastCSharp.net.returnValue<int>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<int, tcpServer._o1>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        getBackupIdentity(null, _onOutput_, true);
                    }
                }


                private void getBackupIdentity(Action<fastCSharp.net.returnValue<tcpServer._o1>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o1>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o1> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o1>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
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
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand _c2 = new fastCSharp.net.tcp.commandClient.identityCommand { Command = 2 + 128, MaxInputSize = 0, IsKeepCallback = 0, IsSendOnly = 0 };

                public void getPermissions(Action<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.pathPermission[]>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.fileTransferServer.server.pathPermission[], tcpServer._o2>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        getPermissions(null, _onOutput_, true);
                    }
                }


                private void getPermissions(Action<fastCSharp.net.returnValue<tcpServer._o2>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o2>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o2> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o2>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            _socket_.Get(_onReturn_, _callback_, _c2, ref _returnType_.Value, _isTask_);
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

                public void list(string path, Action<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.listName[]>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.fileTransferServer.server.listName[], tcpServer._o3>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        list(path, null, _onOutput_, true);
                    }
                }


                private void list(string path, Action<fastCSharp.net.returnValue<tcpServer._o3>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o3>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o3> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o3>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i3 _inputParameter_ = new tcpServer._i3
                            {
                                path = path,
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

                public void delete(string path, fastCSharp.demo.fileTransferServer.server.listName[] listNames, int backupIdentity, Action<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.listName[]>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.fileTransferServer.server.listName[], tcpServer._o4>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        delete(path, listNames, backupIdentity, null, _onOutput_, true);
                    }
                }


                private void delete(string path, fastCSharp.demo.fileTransferServer.server.listName[] listNames, int backupIdentity, Action<fastCSharp.net.returnValue<tcpServer._o4>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o4>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o4> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o4>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i4 _inputParameter_ = new tcpServer._i4
                            {
                                path = path,
                                listNames = listNames,
                                backupIdentity = backupIdentity,
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

                public void upload(fastCSharp.demo.fileTransferServer.server.listName listName, System.IO.Stream fileStream, int backupIdentity, bool isTimeVersion, Action<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.fileState>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o5>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.fileTransferServer.server.fileState, tcpServer._o5>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        upload(listName, fileStream, backupIdentity, isTimeVersion, null, _onOutput_, true);
                    }
                }


                private void upload(fastCSharp.demo.fileTransferServer.server.listName listName, System.IO.Stream fileStream, int backupIdentity, bool isTimeVersion, Action<fastCSharp.net.returnValue<tcpServer._o5>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o5>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o5> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o5>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i5 _inputParameter_ = new tcpServer._i5
                            {
                                listName = listName,
                                fileStream = TcpCommandClient.GetTcpStream(fileStream),
                                backupIdentity = backupIdentity,
                                isTimeVersion = isTimeVersion,
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

                public void upload(fastCSharp.demo.fileTransferServer.server.listName listName, byte[] data, int backupIdentity, bool isTimeVersion, Action<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.fileState>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.fileTransferServer.server.fileState, tcpServer._o6>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        upload(listName, data, backupIdentity, isTimeVersion, null, _onOutput_, true);
                    }
                }


                private void upload(fastCSharp.demo.fileTransferServer.server.listName listName, byte[] data, int backupIdentity, bool isTimeVersion, Action<fastCSharp.net.returnValue<tcpServer._o6>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o6>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o6> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o6>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i6 _inputParameter_ = new tcpServer._i6
                            {
                                listName = listName,
                                data = data,
                                backupIdentity = backupIdentity,
                                isTimeVersion = isTimeVersion,
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

                public void createDirectory(string path, Action<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.fileState>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o7>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.fileTransferServer.server.fileState, tcpServer._o7>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        createDirectory(path, null, _onOutput_, true);
                    }
                }


                private void createDirectory(string path, Action<fastCSharp.net.returnValue<tcpServer._o7>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o7>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o7> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o7>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i7 _inputParameter_ = new tcpServer._i7
                            {
                                path = path,
                            };
                            _socket_.Get(_onReturn_, _callback_, _c7, ref _inputParameter_, ref _returnType_.Value, _isTask_);
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

                public void download(fastCSharp.demo.fileTransferServer.server.listName listName, System.IO.Stream fileStream, Action<fastCSharp.net.returnValue<fastCSharp.demo.fileTransferServer.server.listName>> _onReturn_)
                {
                    fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o8>> _onOutput_;
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn<fastCSharp.demo.fileTransferServer.server.listName, tcpServer._o8>.Get(_onReturn_);
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        download(listName, fileStream, null, _onOutput_, true);
                    }
                }


                private void download(fastCSharp.demo.fileTransferServer.server.listName listName, System.IO.Stream fileStream, Action<fastCSharp.net.returnValue<tcpServer._o8>> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue<tcpServer._o8>> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue<tcpServer._o8> _returnType_ = new fastCSharp.net.returnValue<tcpServer._o8>();
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = TcpCommandClient.StreamSocket;
                        if (_socket_ != null)
                        {
                            tcpServer._i8 _inputParameter_ = new tcpServer._i8
                            {
                                listName = listName,
                                fileStream = TcpCommandClient.GetTcpStream(fileStream),
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
            }

        }
}
#endif