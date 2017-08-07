using System;
#pragma warning disable 162
#pragma warning disable 649

namespace fastCSharp.code.cSharp.template
{
    class tcpServer : pub
    {
        #region NOTE
        private static FullName[] MethodIndexs = null;
        private static FullName[] GenericParameters = null;
        private static FullName ParameterName = null;
        private static FullName ParameterJoinRefName = null;
        private static ParameterTypeRefName ParameterRefName = null;
        private const int InputParameterMaxLength = 0;
        private const int ParameterIndex = 0;
        private const int MethodIndex = 0;
        private const int MaxCommandLength = 0;
        private const int CommandStartIndex = 0;
        private const byte IsClientSendOnly = 0;
        private const byte IsKeepCallback = 0;
        private const bool IsHttpPostOnly = false;
        private const bool IsClientCallbackTask = false;
        private const int LoadBalancingTryCount = 0;
        private const int GroupId = 0;
        #endregion NOTE

        #region PART CLASS
        /*NOTE*/
        public partial class /*NOTE*/@TypeNameDefinition/*IF:IsSetCommandServer*//*IF:IsServerCode*/ : fastCSharp.code.cSharp.tcpServer.ICommandServer/*IF:IsServerCode*//*IF:IsSetCommandServer*/
        {
            #region NOTE
            public void SetCommandServer(fastCSharp.net.tcp.commandServer commandServer) { }
            #endregion NOTE

            #region NOT IsRemember
            /// <summary>
            /// @Attribute.ServiceName TCP服务/*NOT:IsServerCode*/参数/*NOT:IsServerCode*/
            /// </summary>
            public sealed class tcpServer/*IF:IsServerCode*/ : fastCSharp.net.tcp.commandServer/*IF:IsServerCode*/
            {
                #region IF IsServerCode
                private readonly @type.FullName _value_/*NOTE*/ = null/*NOTE*/;
                /// <summary>
                /// @Attribute.ServiceName TCP调用服务端
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                #region NOT IsVerifyMethod
                /// <param name="verify">TCP验证实例</param>
                #endregion NOT IsVerifyMethod
                #region IF type.Type.IsPublic
                /// <param name="value">TCP服务目标对象</param>
                #endregion IF type.Type.IsPublic
                public tcpServer(fastCSharp.code.cSharp.tcpServer attribute = null/*NOT:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null/*NOT:IsVerifyMethod*//*IF:type.Type.IsPublic*/, @type.FullName value = null/*IF:type.Type.IsPublic*/)
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("@Attribute.ServiceName", typeof(@type.FullName))/*NOT:IsVerifyMethod*/, verify/*IF:Attribute.VerifyType*/ ?? new @TcpVerifyType()/*IF:Attribute.VerifyType*//*NOT:IsVerifyMethod*/)
                {
                    _value_ =/*IF:type.Type.IsPublic*/ value ?? /*IF:type.Type.IsPublic*/new @type.FullName();
                    #region IF Attribute.IsIdentityCommand
                    setCommands(@MethodIndexs.Length);
                    #region LOOP MethodIndexs
                    #region NOT IsNullMethod
                    identityOnCommands[/*IF:IsVerifyMethod*/verifyCommandIdentity = /*IF:IsVerifyMethod*/@MethodIndex + @CommandStartIndex].Set(@MethodIndex + @CommandStartIndex/*IF:IsInputParameterMaxLength*/, @InputParameterMaxLength/*IF:IsInputParameterMaxLength*/);
                    #endregion NOT IsNullMethod
                    #endregion LOOP MethodIndexs
                    #endregion IF Attribute.IsIdentityCommand
                    #region NOT Attribute.IsIdentityCommand
                    int commandIndex;
                    keyValue<byte[][], command[]> onCommands = getCommands(@MethodIndexs.Length, out commandIndex);
                    #region LOOP MethodIndexs
                    #region IF IsNullMethod
                    onCommands.Key[commandIndex] = formatMethodKeyName("-@MethodIndexName");
                    onCommands.Value[commandIndex++] = default(command);
                    #endregion IF IsNullMethod
                    #region NOT IsNullMethod
                    onCommands.Key[commandIndex] = /*IF:IsVerifyMethod*/verifyCommand = /*IF:IsVerifyMethod*/formatMethodKeyName("@Method.MethodKeyName");
                    onCommands.Value[commandIndex++].Set(@MethodIndex + @CommandStartIndex/*IF:IsInputParameterMaxLength*/, @InputParameterMaxLength/*IF:IsInputParameterMaxLength*/);
                    #endregion NOT IsNullMethod
                    #endregion LOOP MethodIndexs
                    maxCommandLength = @MaxCommandLength;
                    this.onCommands = new fastCSharp.stateSearcher.ascii<command>(onCommands.Key, onCommands.Value);
                    #endregion NOT Attribute.IsIdentityCommand
                    #region IF ServiceAttribute.IsHttpClient
                    int httpCount = @MethodIndexs.Length;
                    string[] httpNames = new string[httpCount];
                    httpCommand[] httpCommands = new httpCommand[httpCount];
                    #region LOOP MethodIndexs
                    #region IF IsNullMethod
                    httpNames[--httpCount] = "-@MethodIndexName";
                    httpCommands[httpCount] = default(httpCommand);
                    #endregion IF IsNullMethod
                    #region NOT IsNullMethod
                    /*IF:IsVerifyMethod*/
                    verifyCommand = /*IF:IsVerifyMethod*/fastCSharp.String.getBytes(httpNames[--httpCount] = "@HttpMethodName");
                    httpCommands[httpCount].Set(@MethodIndex + @CommandStartIndex/*PUSH:Attribute*/, @IsHttpPostOnly/*PUSH:Attribute*//*IF:IsInputParameterMaxLength*/, @InputParameterMaxLength/*IF:IsInputParameterMaxLength*/);
                    #endregion NOT IsNullMethod
                    #endregion LOOP MethodIndexs
                    maxCommandLength = @MaxCommandLength;
                    this.httpCommands = new fastCSharp.stateSearcher.ascii<httpCommand>(httpNames, httpCommands);
                    #endregion IF ServiceAttribute.IsHttpClient
                    #region IF IsSetCommandServer
                    _value_.SetCommandServer(this);
                    #endregion IF IsSetCommandServer
                }
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                /// <param name="data"></param>
                protected override void doCommand(int index, socket socket, ref subArray<byte> data)
                {
                    if (index < @CommandStartIndex) base.doCommand(index, socket, ref data);
                    else
                    {
                        switch (index - @CommandStartIndex)
                        {
                            #region LOOP MethodIndexs
                            #region NOT IsNullMethod
                            case @MethodIndex: @MethodIndexName(socket, ref data); return;
                            #endregion NOT IsNullMethod
                            #endregion LOOP MethodIndexs
                            default: return;
                        }
                    }
                }
                #region IF ServiceAttribute.IsHttpClient
                /// <summary>
                /// 命令处理
                /// </summary>
                /// <param name="index"></param>
                /// <param name="socket"></param>
                protected override void doHttpCommand(int index, fastCSharp.net.tcp.http.socketBase socket)
                {
                    switch (index - @CommandStartIndex)
                    {
                        #region LOOP MethodIndexs
                        #region NOT IsNullMethod
                        case @MethodIndex: @MethodIndexName(socket); return;
                        #endregion NOT IsNullMethod
                        #endregion LOOP MethodIndexs
                        default: return;
                    }
                }
                #endregion IF ServiceAttribute.IsHttpClient
                #region LOOP MethodGroups
                private int @GroupCountName;
                private int @GroupIgnoreName;
                #endregion LOOP MethodGroups
                #region IF MethodGroups.Length
                /// <summary>
                /// 忽略分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                protected override void ignoreGroup(int groupId)
                {
                    switch (groupId)
                    {
                        #region LOOP MethodGroups
                        case @GroupId:
                            @GroupIgnoreName = 1;
                            while (@GroupCountName != 0) System.Threading.Thread.Sleep(1);
                            break;
                        #endregion LOOP MethodGroups
                    }
                }
                #endregion IF MethodGroups.Length
                #region IF IsAnyGenericMethod
                static tcpServer()
                {
                    System.Collections.Generic.Dictionary<fastCSharp.code.cSharp.tcpBase.genericMethod, System.Reflection.MethodInfo> genericMethods = fastCSharp.code.cSharp.tcpServer.GetGenericMethods(typeof(@type.FullName));
                    #region LOOP MethodIndexs
                    #region NOT IsNullMethod
                    #region IF Method.Method.IsGenericMethod
                    @GenericMethodInfoName = /*PUSH:Method*/genericMethods[new fastCSharp.code.cSharp.tcpBase.genericMethod("@MethodName", @GenericParameters.Length/*LOOP:Parameters*/, "@ParameterRef@ParameterType.FullName"/*LOOP:Parameters*/)]/*PUSH:Method*/;
                    #endregion IF Method.Method.IsGenericMethod
                    #endregion NOT IsNullMethod
                    #endregion LOOP MethodIndexs
                }
                #endregion IF IsAnyGenericMethod
                #endregion IF IsServerCode

                #region LOOP MethodIndexs
                #region NOT IsNullMethod
                #region IF IsInputParameter
                [fastCSharp.emit.dataSerialize(IsMemberMap = false/*NOT:Attribute.IsInputSerializeReferenceMember*/, IsReferenceMember = false/*NOT:Attribute.IsInputSerializeReferenceMember*/)]
                #region IF Attribute.IsInputSerializeBox
                [fastCSharp.emit.boxSerialize]
                #endregion IF Attribute.IsInputSerializeBox
                internal /*AT:InputParameterClassType*//*NOTE*/partial class/*NOTE*/ @InputParameterTypeName
                {
                    #region IF Method.Method.IsGenericMethod
                    public fastCSharp.code.remoteType[] @GenericParameterTypeName;
                    #endregion IF Method.Method.IsGenericMethod
                    #region LOOP MethodParameters
                    #region IF ParameterType.IsStream
                    public fastCSharp.code.cSharp.tcpBase.tcpStream @StreamParameterName;
                    #endregion IF ParameterType.IsStream
                    #region NOT ParameterType.IsStream
                    public @GenericParameterType.FullName @ParameterName;
                    #endregion NOT ParameterType.IsStream
                    #endregion LOOP MethodParameters
                    #region IF IsGenericParameterCallback
                    public fastCSharp.code.remoteType @ReturnTypeName;
                    #endregion IF IsGenericParameterCallback
                    #region NOTE
                    public object ParameterJoinName = null;
                    public object ParameterRealName = null;
                    public GenericParameterType.FullName ReturnInputParameterName;
                    #endregion NOTE
                }
                #endregion IF IsInputParameter
                #region IF IsOutputParameter
                [fastCSharp.emit.dataSerialize(IsMemberMap = false/*NOT:Attribute.IsOutputSerializeReferenceMember*/, IsReferenceMember = false/*NOT:Attribute.IsOutputSerializeReferenceMember*/)]
                #region IF Attribute.IsOutputSerializeBox
                [fastCSharp.emit.boxSerialize]
                #endregion IF Attribute.IsOutputSerializeBox
#if NOJIT
                internal /*AT:OutputParameterClassType*//*NOTE*/class/*NOTE*/ @OutputParameterTypeName/*IF:MethodIsReturn*/ : /*IF:Attribute.IsOutputParameterClass*/fastCSharp.net.asynchronousMethod.returnParameter<@GenericReturnType.FullName>/*IF:Attribute.IsOutputParameterClass*//*NOTE*/, /*NOTE*//*NOT:Attribute.IsOutputParameterClass*/fastCSharp.net.asynchronousMethod.IReturnParameter/*NOT:Attribute.IsOutputParameterClass*//*IF:MethodIsReturn*/
#else
                internal /*AT:OutputParameterClassType*//*NOTE*/class/*NOTE*/ @OutputParameterTypeName/*IF:MethodIsReturn*/ : /*IF:Attribute.IsOutputParameterClass*/fastCSharp.net.asynchronousMethod.returnParameter<@GenericReturnType.FullName>/*IF:Attribute.IsOutputParameterClass*//*NOTE*/, /*NOTE*//*NOT:Attribute.IsOutputParameterClass*/fastCSharp.net.asynchronousMethod.IReturnParameter<@GenericReturnType.FullName>/*NOT:Attribute.IsOutputParameterClass*//*IF:MethodIsReturn*/
#endif
                {
                    #region LOOP Method.OutputParameters
                    public @GenericParameterType.FullName @ParameterName;
                    #endregion LOOP Method.OutputParameters
                    #region IF MethodIsReturn
                    #region NOT Attribute.IsOutputParameterClass
                    [fastCSharp.emit.jsonSerialize.member(IsIgnoreCurrent = true)]
                    [fastCSharp.emit.jsonParse.member(IsIgnoreCurrent = true)]
                    public @GenericReturnType.FullName Ret;
                    public /*NOTE*/new /*NOTE*/@GenericReturnType.FullName Return
                    {
                        get { return Ret; }
                        set { Ret = value; }
                    }
#if NOJIT
                    public object ReturnObject
                    {
                        get { return Ret; }
                        set { Ret = (@GenericReturnType.FullName)value; }
                    }
#endif
                    #endregion NOT Attribute.IsOutputParameterClass
                    #endregion IF MethodIsReturn
                    #region NOTE
                    public GenericReturnType.FullName ReturnName;
                    #endregion NOTE
                }
                #endregion IF IsOutputParameter
                #region IF IsServerCode
                #region IF Method.Method.IsGenericMethod
                private static readonly System.Reflection.MethodInfo @GenericMethodInfoName;
                #endregion IF Method.Method.IsGenericMethod
                #region NOT IsAsynchronousCallback
                #region IF IsMethodServerCall
                sealed class @MethodStreamName : fastCSharp.net.tcp.commandServer.serverCall<@MethodStreamName, @type.FullName/*IF:IsInputParameter*/, @InputParameterTypeName/*IF:IsInputParameter*/>
                {
                    private void get(ref fastCSharp.net.returnValue/*IF:IsOutputParameter*/<@OutputParameterTypeName>/*IF:IsOutputParameter*/ value)
                    {
                        try
                        {
                            #region IF IsInputParameter
                            #region IF Method.Method.IsGenericMethod
                            object[] invokeParameter = new object[] { /*IF:ClientParameterName*/socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/};
                            #endregion IF Method.Method.IsGenericMethod
                            #endregion IF IsInputParameter
                            /*IF:MethodIsReturn*/
                            @GenericReturnType.FullName @ReturnName;/*IF:MethodIsReturn*/

                            #region IF MemberIndex
                            #region IF Method.IsGetMember
                            #region IF Method.PropertyParameters.Length
                            @ReturnName = /*NOTE*/(GenericReturnType.FullName)/*NOTE*/serverValue[/*IF:ClientParameterName*/socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/];
                            #endregion IF Method.PropertyParameters.Length
                            #region NOT Method.PropertyParameters.Length
                            @ReturnName = /*NOTE*/(GenericReturnType.FullName)/*NOTE*/serverValue.@PropertyName;
                            #endregion NOT Method.PropertyParameters.Length
                            #endregion IF Method.IsGetMember
                            #region NOT Method.IsGetMember
                            #region IF Method.PropertyParameters.Length
                            serverValue[/*IF:ClientParameterName*/socket, /*IF:ClientParameterName*//*LOOP:Method.PropertyParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:Method.PropertyParameters*/] = /*PUSH:Method.PropertyParameter*/inputParameter.@ParameterName/*PUSH:Method.PropertyParameter*/;
                            #endregion IF Method.PropertyParameters.Length
                            #region NOT Method.PropertyParameters.Length
                            serverValue.@PropertyName = /*LOOP:MethodParameters*/inputParameter.@ParameterName/*LOOP:MethodParameters*/;
                            #endregion NOT Method.PropertyParameters.Length
                            #endregion NOT Method.IsGetMember
                            #endregion IF MemberIndex

                            #region NOT MemberIndex
                            #region IF Method.Method.IsGenericMethod
                            /*IF:MethodIsReturn*/
                            @ReturnName =  (@GenericReturnType.FullName)/*IF:MethodIsReturn*/fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(serverValue, @GenericMethodInfoName, inputParameter.@GenericParameterTypeName/*IF:IsInputParameter*/, invokeParameter/*IF:IsInputParameter*/);
                            #endregion IF Method.Method.IsGenericMethod
                            #region NOT Method.Method.IsGenericMethod
                            /*IF:MethodIsReturn*/
                            @ReturnName = /*NOTE*/(GenericReturnType.FullName)/*NOTE*//*IF:MethodIsReturn*//*PUSH:Method*/serverValue.@MethodGenericName/*PUSH:Method*/(/*IF:ClientParameterName*/socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/);
                            #endregion NOT Method.Method.IsGenericMethod
                            #endregion NOT MemberIndex

                            #region IF IsOutputParameter
                            #region IF Method.Method.IsGenericMethod
                            #region LOOP MethodParameters
                            #region IF IsRefOrOut
                            inputParameter.@ParameterName = (@GenericParameterType.FullName)invokeParameter[@ParameterIndex];
                            #endregion IF IsRefOrOut
                            #endregion LOOP MethodParameters
                            #endregion IF Method.Method.IsGenericMethod
                            #region IF IsVerifyMethod
                            if (/*NOTE*/(bool)(object)/*NOTE*/@ReturnName) socket.SetVerifyMethod();
                            #endregion IF IsVerifyMethod
                            value.Value = new @OutputParameterTypeName
                            {
                                #region LOOP Method.OutputParameters
                                @ParameterName = inputParameter.@ParameterName,
                                #endregion LOOP Method.OutputParameters
                                #region IF MethodIsReturn
                                @ReturnName = @ReturnName
                                #endregion IF MethodIsReturn
                            };
                            #endregion IF IsOutputParameter
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
                        fastCSharp.net.returnValue/*IF:IsOutputParameter*/<@OutputParameterTypeName>/*IF:IsOutputParameter*/ value = new fastCSharp.net.returnValue/*IF:IsOutputParameter*/<@OutputParameterTypeName>/*IF:IsOutputParameter*/();
                        #region IF IsClientSendOnly
                        if (isVerify == 0) get(ref value);
                        #endregion IF IsClientSendOnly
                        #region NOT IsClientSendOnly
                        if (isVerify == 0)
                        {
                            get(ref value);
                            socket.SendStream(ref identity, ref value, flags);
                        }
                        #endregion NOT IsClientSendOnly
                        fastCSharp.typePool<@MethodStreamName>.PushNotNull(this);
                    }
                }
                #endregion IF IsMethodServerCall
                #region IF ServiceAttribute.IsHttpClient
                struct @MethodMergeName
                {
                    public @type.FullName ServerValue;
                    public socket Socket;
                    #region IF IsInputParameter
                    public @InputParameterTypeName InputParameter;
                    #endregion IF IsInputParameter
                    public fastCSharp.net.returnValue/*IF:IsOutputParameter*/<@OutputParameterTypeName>/*IF:IsOutputParameter*/ Get()
                    {
                        fastCSharp.net.returnValue.type returnType;
                        try
                        {
                            #region IF IsInputParameter
                            #region IF Method.Method.IsGenericMethod
                            object[] invokeParameter = new object[] { /*IF:ClientParameterName*/Socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/Socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*/InputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/};
                            #endregion IF Method.Method.IsGenericMethod
                            #endregion IF IsInputParameter
                            /*IF:MethodIsReturn*/
                            @GenericReturnType.FullName @ReturnName = /*IF:MethodIsReturn*/
                            #region IF MemberIndex
                            #region IF Method.IsGetMember
                            #region IF Method.PropertyParameters.Length
                            @ReturnName = /*NOTE*/(GenericReturnType.FullName)/*NOTE*/ServerValue[/*IF:ClientParameterName*/Socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/Socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/InputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/];
                            #endregion IF Method.PropertyParameters.Length
                            #region NOT Method.PropertyParameters.Length
                            @ReturnName = /*NOTE*/(GenericReturnType.FullName)/*NOTE*/ServerValue.@PropertyName;
                            #endregion NOT Method.PropertyParameters.Length
                            #endregion IF Method.IsGetMember
                            #region NOT Method.IsGetMember
                            #region IF Method.PropertyParameters.Length
                            ServerValue[/*IF:ClientParameterName*/Socket, /*IF:ClientParameterName*//*LOOP:Method.PropertyParameters*//*IF:ParameterType.IsStream*/Socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/InputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:Method.PropertyParameters*/] = /*PUSH:Method.PropertyParameter*/InputParameter.@ParameterName/*PUSH:Method.PropertyParameter*/;
                            #endregion IF Method.PropertyParameters.Length
                            #region NOT Method.PropertyParameters.Length
                            ServerValue.@PropertyName = /*LOOP:MethodParameters*/InputParameter.@ParameterName/*LOOP:MethodParameters*/;
                            #endregion NOT Method.PropertyParameters.Length
                            #endregion NOT Method.IsGetMember
                            #endregion IF MemberIndex
                            #region NOT MemberIndex
                            #region IF Method.Method.IsGenericMethod
                            /*IF:MethodIsReturn*/
                            @ReturnName = /*IF:MethodIsReturn*//*IF:MethodIsReturn*/(@GenericReturnType.FullName)/*IF:MethodIsReturn*/fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(ServerValue, @GenericMethodInfoName, InputParameter.@GenericParameterTypeName/*IF:IsInputParameter*/, invokeParameter/*IF:IsInputParameter*/);
                            #endregion IF Method.Method.IsGenericMethod
                            #region NOT Method.Method.IsGenericMethod
                            /*IF:MethodIsReturn*/
                            @ReturnName = /*NOTE*/(GenericReturnType.FullName)/*NOTE*//*IF:MethodIsReturn*//*PUSH:Method*/ServerValue.@MethodGenericName/*PUSH:Method*/(/*IF:ClientParameterName*/Socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/Socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/InputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/);
                            #endregion NOT Method.Method.IsGenericMethod
                            #endregion NOT MemberIndex
                            
                            #region IF IsOutputParameter
                            #region IF Method.Method.IsGenericMethod
                            #region LOOP MethodParameters
                            #region IF IsRefOrOut
                            InputParameter.@ParameterName = (@GenericParameterType.FullName)invokeParameter[@ParameterIndex];
                            #endregion IF IsRefOrOut
                            #endregion LOOP MethodParameters
                            #endregion IF Method.Method.IsGenericMethod
                            #region IF IsVerifyMethod
                            if (/*NOTE*/(bool)(object)/*NOTE*/@ReturnName) Socket.SetVerifyMethod();
                            #endregion IF IsVerifyMethod
                            return new @OutputParameterTypeName
                            {
                                #region LOOP Method.OutputParameters
                                @ParameterName = InputParameter.@ParameterName,
                                #endregion LOOP Method.OutputParameters
                                #region IF MethodIsReturn
                                @ReturnName = @ReturnName
                                #endregion IF MethodIsReturn
                            };
                            #endregion IF IsOutputParameter
                            #region NOT IsOutputParameter
                            return new fastCSharp.net.returnValue/*IF:IsOutputParameter*/<@OutputParameterTypeName>/*IF:IsOutputParameter*/{ Type = fastCSharp.net.returnValue.type.Success };
                            #endregion NOT IsOutputParameter
                        }
                        catch (Exception error)
                        {
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        return new fastCSharp.net.returnValue/*IF:IsOutputParameter*/<@OutputParameterTypeName>/*IF:IsOutputParameter*/{ Type = returnType };
                    }
                }
                #endregion IF ServiceAttribute.IsHttpClient
                #endregion NOT IsAsynchronousCallback
                private void @MethodIndexName(socket socket, ref subArray<byte> data)
                {
                    #region IF Attribute.IsExpired
                    #region NOT IsClientSendOnly
                    socket.SendStream(socket.Identity, fastCSharp.net.returnValue.type.VersionExpired);
                    #endregion NOT IsClientSendOnly
                    #endregion IF Attribute.IsExpired
                    #region NOT Attribute.IsExpired
                    #region NOT IsClientSendOnly
                    fastCSharp.net.returnValue.type returnType = fastCSharp.net.returnValue.type.Unknown;
                    #endregion NOT IsClientSendOnly
                    #region IF Attribute.GroupId
                    if (@GroupIgnoreName == 0)
                    {
                        System.Threading.Interlocked.Increment(ref @GroupCountName);
                    #endregion IF Attribute.GroupId
                        try
                        {
                            #region IF IsInputParameter
                            @InputParameterTypeName inputParameter = new @InputParameterTypeName();
                            if ((socket.Flags & fastCSharp.net.tcp.commandServer.commandFlags.JsonSerialize) == 0 ? fastCSharp.emit.dataDeSerializer.DeSerialize(ref data, ref inputParameter) : fastCSharp.code.cSharp.tcpBase.JsonDeSerialize(ref inputParameter, ref data))
                            #endregion IF IsInputParameter
                            {
                                #region IF IsAsynchronousCallback
                                #region IF MethodIsReturn
                                @OutputParameterTypeName outputParameter = new @OutputParameterTypeName();
                                Func<fastCSharp.net.returnValue<@GenericReturnType.FullName>, bool> callbackReturn = fastCSharp.net.tcp.commandServer.socket.callback<@OutputParameterTypeName/*NOT:IsVerifyMethod*/, @GenericReturnType.FullName/*NOT:IsVerifyMethod*/>.Get/*AT:KeepCallback*/(socket, ref outputParameter, @IsClientSendOnly);
                                if (callbackReturn != null)
                                {
                                    #region IF IsInvokeGenericMethod
                                    object[] invokeParameter = new object[] { /*IF:ClientParameterName*/socket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/, /*LOOP:MethodParameters*//*IF:IsGenericParameterCallback*/fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.@ReturnTypeName, /*IF:IsGenericParameterCallback*//*NOTE*/(Func<fastCSharp.net.returnValue<object>, bool>)(object)/*NOTE*//*NOT:IsGenericParameterCallback*/(Func<fastCSharp.net.returnValue<@GenericReturnType.FullName>, bool>)/*NOT:IsGenericParameterCallback*/callbackReturn/*IF:IsGenericParameterCallback*/)/*IF:IsGenericParameterCallback*/ };
                                    #endregion IF IsInvokeGenericMethod
                                    #region IF Method.Method.IsGenericMethod
                                    fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(_value_, @GenericMethodInfoName, inputParameter.@GenericParameterTypeName, invokeParameter);
                                    #endregion IF Method.Method.IsGenericMethod
                                    #region NOT Method.Method.IsGenericMethod
                                    /*PUSH:Method*/
                                    _value_.@MethodGenericName/*PUSH:Method*/(/*IF:ClientParameterName*/socket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue<MethodReturnType.FullName>, bool>)(object)(Func<fastCSharp.net.returnValue<@GenericReturnType.FullName>, bool>)/*NOTE*/callbackReturn);
                                    #endregion NOT Method.Method.IsGenericMethod
                                }
                                #endregion IF MethodIsReturn
                                #region NOT MethodIsReturn
                                Func<fastCSharp.net.returnValue, bool> callback/*IF:IsClientSendOnly*/ = null/*IF:IsClientSendOnly*/;
                                #region NOT IsClientSendOnly
                                if ((callback = fastCSharp.net.tcp.commandServer.socket.callback.Get/*AT:KeepCallback*/(socket, @IsClientSendOnly)) != null)
                                #endregion NOT IsClientSendOnly
                                {
                                    #region IF IsInvokeGenericMethod
                                    object[] invokeParameter = new object[] { /*IF:ClientParameterName*/socket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/, /*LOOP:MethodParameters*//*IF:IsGenericParameterCallback*/fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.@ReturnTypeName, /*IF:IsGenericParameterCallback*//*NOTE*/(Func<fastCSharp.net.returnValue<object>, bool>)(object)/*NOTE*//*NOT:IsGenericParameterCallback*/(Func<fastCSharp.net.returnValue, bool>)/*NOT:IsGenericParameterCallback*/callback/*IF:IsGenericParameterCallback*/)/*IF:IsGenericParameterCallback*/ };
                                    #endregion IF IsInvokeGenericMethod
                                    #region IF Method.Method.IsGenericMethod
                                    fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(_value_, @GenericMethodInfoName, inputParameter.@GenericParameterTypeName, invokeParameter);
                                    #endregion IF Method.Method.IsGenericMethod
                                    #region NOT Method.Method.IsGenericMethod
                                    /*PUSH:Method*/
                                    _value_.@MethodGenericName/*PUSH:Method*/(/*IF:ClientParameterName*/socket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue<MethodReturnType.FullName>, bool>)(object)(Func<fastCSharp.net.returnValue, bool>)/*NOTE*/callback);
                                    #endregion NOT Method.Method.IsGenericMethod
                                }
                                #endregion NOT MethodIsReturn
                                #endregion IF IsAsynchronousCallback
                                #region NOT IsAsynchronousCallback
                                #region IF IsMethodServerCall
                                #region IF IsServerAsynchronousTask
                                fastCSharp.threading.task.Tiny.Add(@MethodStreamName/**/.GetCall(socket, _value_/*IF:IsInputParameter*/, ref inputParameter/*IF:IsInputParameter*/ ));
                                #endregion IF IsServerAsynchronousTask
                                #region NOT IsServerAsynchronousTask
                                @MethodStreamName/**/.Call(socket, _value_/*IF:IsInputParameter*/, ref inputParameter/*IF:IsInputParameter*/ );
                                #endregion NOT IsServerAsynchronousTask
                                #endregion IF IsMethodServerCall
                                #region NOT IsMethodServerCall
                                /*PUSH:Method*/
                                _value_.@MethodGenericName/*PUSH:Method*/(/*IF:ClientParameterName*/socket/*IF:ClientParameterName*//*LOOP:MethodParameters*/, /*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*LOOP:MethodParameters*/);
                                #endregion NOT IsMethodServerCall
                                #endregion NOT IsAsynchronousCallback
                                return;
                            }
                            #region IF IsInputParameter
                            #region NOT IsClientSendOnly
                            returnType = fastCSharp.net.returnValue.type.ServerDeSerializeError;
                            #endregion NOT IsClientSendOnly
                            #endregion IF IsInputParameter
                        }
                        catch (Exception error)
                        {
                            #region NOT IsClientSendOnly
                            returnType = fastCSharp.net.returnValue.type.ServerException;
                            #endregion NOT IsClientSendOnly
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        #region IF Attribute.GroupId
                        finally
                        {
                            System.Threading.Interlocked.Decrement(ref @GroupCountName);
                        }
                    }
                        #endregion IF Attribute.GroupId
                    #region NOT IsClientSendOnly
                    socket.SendStream(socket.Identity, returnType);
                    #endregion NOT IsClientSendOnly
                    #endregion NOT Attribute.IsExpired
                }
                #region IF ServiceAttribute.IsHttpClient
                private void @MethodIndexName(fastCSharp.net.tcp.http.socketBase socket)
                {
                    #region IF Attribute.IsExpired
                    socket.ResponseError(socket.TcpCommandSocket.HttpPage.SocketIdentity, fastCSharp.net.tcp.http.response.state.NotFound404);
                    #endregion IF Attribute.IsExpired
                    #region NOT Attribute.IsExpired
                    long identity = int.MinValue;
                    #region IF Attribute.GroupId
                    if (@GroupIgnoreName == 0)
                    {
                        System.Threading.Interlocked.Increment(ref @GroupCountName);
                    #endregion IF Attribute.GroupId
                        try
                        {
                            socket commandSocket = socket.TcpCommandSocket;
                            fastCSharp.code.cSharp.tcpBase.httpPage httpPage = commandSocket.HttpPage;
                            identity = httpPage.SocketIdentity;
                            #region IF IsInputParameter
                            @InputParameterTypeName inputParameter = new @InputParameterTypeName();
                            if (httpPage.DeSerialize(ref inputParameter))
                            #endregion IF IsInputParameter
                            {
                                #region IF IsAsynchronousCallback
                                #region IF IsInvokeGenericMethod
                                object[] invokeParameter;
                                #region IF MethodIsReturn
                                @OutputParameterTypeName invokeOutputParameter = new @OutputParameterTypeName();
                                invokeParameter = new object[] { /*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*IF:IsGenericParameterCallback*/fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.@ReturnTypeName, /*IF:IsGenericParameterCallback*//*NOTE*/(Func<fastCSharp.net.returnValue<object>, bool>)(object)/*NOTE*//*NOT:IsGenericParameterCallback*/(Func<fastCSharp.net.returnValue<@GenericReturnType.FullName>, bool>)/*NOT:IsGenericParameterCallback*/fastCSharp.net.tcp.commandServer.socket.callbackHttp/*IF:IsOutputParameter*//*IF:IsOutputParameter*//*IF:IsVerifyMethod*//*AT:IsVerifyMethod*//*IF:IsVerifyMethod*/<@OutputParameterTypeName, @GenericReturnType.FullName>.Get(httpPage, ref invokeOutputParameter)/*IF:IsGenericParameterCallback*/)/*IF:IsGenericParameterCallback*/ };
                                #endregion IF MethodIsReturn
                                #region NOT MethodIsReturn
                                invokeParameter = new object[] { /*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*IF:IsGenericParameterCallback*/fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.@ReturnTypeName, /*IF:IsGenericParameterCallback*//*NOTE*/(Func<fastCSharp.net.returnValue<object>, bool>)(object)/*NOTE*//*NOT:IsGenericParameterCallback*/(Func<fastCSharp.net.returnValue, bool>)/*NOT:IsGenericParameterCallback*/fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage)/*IF:IsGenericParameterCallback*/)/*IF:IsGenericParameterCallback*/ };
                                #endregion NOT MethodIsReturn
                                #endregion IF IsInvokeGenericMethod
                                #region IF Method.Method.IsGenericMethod
                                fastCSharp.code.cSharp.tcpServer.InvokeGenericMethod(_value_, @GenericMethodInfoName, inputParameter.@GenericParameterTypeName, invokeParameter);
                                #endregion IF Method.Method.IsGenericMethod
                                #region NOT Method.Method.IsGenericMethod
                                #region IF MethodIsReturn
                                @OutputParameterTypeName outputParameter = new @OutputParameterTypeName();
                                /*PUSH:Method*/
                                _value_.@MethodGenericName/*PUSH:Method*/(/*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue<MethodReturnType.FullName>, bool>)(object)(Func<fastCSharp.net.returnValue<@GenericReturnType.FullName>, bool>)/*NOTE*/fastCSharp.net.tcp.commandServer.socket.callbackHttp/*IF:IsOutputParameter*//*IF:IsOutputParameter*//*IF:IsVerifyMethod*//*AT:IsVerifyMethod*//*IF:IsVerifyMethod*/<@OutputParameterTypeName, @GenericReturnType.FullName>.Get(httpPage, ref outputParameter));
                                #endregion IF MethodIsReturn
                                #region NOT MethodIsReturn
                                /*PUSH:Method*/
                                _value_.@MethodGenericName/*PUSH:Method*/(/*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*AT:ParameterRef*//*IF:IsGenericParameter*/(@ParameterType.FullName)/*IF:IsGenericParameter*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue<MethodReturnType.FullName>, bool>)(object)(Func<fastCSharp.net.returnValue, bool>)/*NOTE*/fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage));
                                #endregion NOT MethodIsReturn
                                #endregion NOT Method.Method.IsGenericMethod
                                #endregion IF IsAsynchronousCallback
                                #region NOT IsAsynchronousCallback
                                httpPage.Response(new @MethodMergeName { Socket = commandSocket, ServerValue = _value_/*IF:IsInputParameter*/, InputParameter = inputParameter/*IF:IsInputParameter*/ }.Get());
                                #endregion NOT IsAsynchronousCallback
                                return;
                            }
                        }
                        catch (Exception error)
                        {
                            fastCSharp.log.Error.Add(error, null, true);
                        }
                        #region IF Attribute.GroupId
                        finally
                        {
                            System.Threading.Interlocked.Decrement(ref @GroupCountName);
                        }
                    }
                        #endregion IF Attribute.GroupId
                    socket.ResponseError(identity, fastCSharp.net.tcp.http.response.state.ServerError500);
                    #endregion NOT Attribute.IsExpired
                }
                #endregion IF ServiceAttribute.IsHttpClient
                #endregion IF IsServerCode
                #endregion NOT IsNullMethod
                #endregion LOOP MethodIndexs
            }
            #region IF IsClientCode
            #region IF Attribute.IsLoadBalancing
            /// <summary>
            /// TCP负载均衡服务
            /// </summary>
            public sealed class tcpLoadBalancing : fastCSharp.net.tcp.commandLoadBalancingServer<tcpClient>
            {
                /// <summary>
                /// TCP负载均衡服务
                /// </summary>
                /// <param name="attribute">TCP调用服务器端配置信息</param>
                #region IF IsVerifyMethod
                /// <param name="verifyMethod">TCP验证方法</param>
                #endregion IF IsVerifyMethod
                #region NOT IsVerifyMethod
                /// <param name="verify">TCP验证实例</param>
                #endregion NOT IsVerifyMethod
#if NOJIT
                public tcpLoadBalancing(fastCSharp.code.cSharp.tcpServer attribute = null/*IF:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null/*NOT:IsVerifyMethod*/)
#else
                public tcpLoadBalancing(fastCSharp.code.cSharp.tcpServer attribute = null/*IF:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null/*NOT:IsVerifyMethod*/)
#endif
                    : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("@LoadBalancingServiceName", typeof(@type.FullName))/*IF:IsVerifyMethod*/, verifyMethod/*IF:Attribute.VerifyMethodType*/ ?? new @TcpVerifyMethodType()/*IF:Attribute.VerifyMethodType*//*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, verify/*IF:Attribute.VerifyType*/ ?? new @TcpVerifyType()/*IF:Attribute.VerifyType*//*NOT:IsVerifyMethod*/)
                {
                }
                protected override tcpClient _createClient_(fastCSharp.code.cSharp.tcpServer attribute)
                {
                    tcpClient client = new tcpClient(attribute/*IF:IsVerifyMethod*/, _verifyMethod_/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, _verify_/*NOT:IsVerifyMethod*/);
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

#region LOOP MethodIndexs
#region NOT IsNullMethod
#region IF IsClientSynchronous
                public fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/ /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterJoinName/*LOOP:MethodParameters*/)
                {
                    fastCSharp.net.returnValue.type _returnType_;
#region IF Attribute.IsExpired
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;
#region IF IsOutputParameter
#region LOOP MethodParameters
#region IF IsOut
                    @ParameterName = default(@ParameterType.FullName);
#endregion IF IsOut
#endregion LOOP MethodParameters
#endregion IF IsOutputParameter
#endregion IF Attribute.IsExpired
#region NOT Attribute.IsExpired
                    _returnType_ = fastCSharp.net.returnValue.type.Unknown;
                    int _tryCount_ = /*PUSH:ServiceAttribute*/@LoadBalancingTryCount/*PUSH:ServiceAttribute*/;
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
                            fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/ _return_ = _client_.Client./*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*NOTE*/(ParameterTypeRefName)(object)/*NOTE*//*LOOP:MethodParameters*/@ParameterJoinRefName/*LOOP:MethodParameters*/);
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
#endregion NOT Attribute.IsExpired
                    return new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = _returnType_ };
                }
#endregion IF IsClientSynchronous
#region IF IsClientAsynchronous
#region NOT IsKeepCallback
                sealed class @LoadBalancingCallbackName : fastCSharp.code.cSharp.tcpBase.loadBalancingCallback/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/
                {
                    private clientIdentity _client_;
                    private tcpLoadBalancing _loadBalancingServer_;
#region LOOP MethodParameters
                    private @ParameterType.FullName @ParameterName;
#endregion LOOP MethodParameters
                    protected override void _call_()
                    {
                        fastCSharp.net.returnValue.type _returnType_;
                        try
                        {
                            _client_ = _loadBalancingServer_._getClient_();
                            if (_client_.Client != null)
                            {
                                _client_.Client./*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterRefName, /*LOOP:MethodParameters*/_onReturnHandle_);
                                return;
                            }
                            _returnType_ = fastCSharp.net.returnValue.type.ClientDisposed;
                        }
                        catch (Exception error)
                        {
                            _loadBalancingServer_._end_(ref _client_, _returnType_ = fastCSharp.net.returnValue.type.ClientException);
                            fastCSharp.log.Error.Add(error, null, false);
                        }
                        _push_(new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = _returnType_ });
                    }
                    protected override void _push_(fastCSharp.net.returnValue.type isReturn)
                    {
                        _loadBalancingServer_._end_(ref _client_, isReturn);
                        _loadBalancingServer_ = null;
#region IF IsOutputParameter
#region LOOP MethodParameters
                        @ParameterName = default(@ParameterType.FullName);
#endregion LOOP MethodParameters
#endregion IF IsOutputParameter
                        fastCSharp.typePool<@LoadBalancingCallbackName>.PushNotNull(this);
                    }
                    public static void _Call_(tcpLoadBalancing _loadBalancingServer_,
                        /*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/> _onReturn_)
                    {
                        @LoadBalancingCallbackName _callback_ = fastCSharp.typePool<@LoadBalancingCallbackName>.Pop();
                        if (_callback_ == null)
                        {
                            try
                            {
                                _callback_ = new @LoadBalancingCallbackName();
                            }
                            catch (Exception error)
                            {
                                fastCSharp.log.Error.Add(error, null, false);
                                _onReturn_(new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = fastCSharp.net.returnValue.type.ClientException });
#region IF IsOutputParameter
#region LOOP MethodParameters
#region IF IsOut
                                @ParameterName = /*NOTE*/(ParameterTypeRefName)(object)/*NOTE*/default(@ParameterType.FullName);
#endregion IF IsOut
#endregion LOOP MethodParameters
#endregion IF IsOutputParameter
                                return;
                            }
                        }
#region LOOP MethodParameters
                        _callback_.@ParameterName = /*NOTE*/(ParameterType.FullName)(object)/*NOTE*/@ParameterName;
#endregion LOOP MethodParameters
                        _callback_._loadBalancingServer_ = _loadBalancingServer_;
                        _callback_._onReturn_ = _onReturn_;
                        _callback_._tryCount_ =/*PUSH:ServiceAttribute*/@LoadBalancingTryCount/*PUSH:ServiceAttribute*/;
#region IF IsOutputParameter
#region LOOP MethodParameters
#region IF IsOut
                        @ParameterName = /*NOTE*/(ParameterTypeRefName)(object)/*NOTE*/default(@ParameterType.FullName);
#endregion IF IsOut
#endregion LOOP MethodParameters
#endregion IF IsOutputParameter
                        _callback_._call_();
                    }
                }
                public void /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/> _onReturn_)
                {
#region IF Attribute.IsExpired
                    _onReturn_(new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = fastCSharp.net.returnValue.type.VersionExpired });
#endregion IF Attribute.IsExpired
#region NOT Attribute.IsExpired
                    @LoadBalancingCallbackName/**/._Call_(this, /*LOOP:MethodParameters*/@ParameterName, /*LOOP:MethodParameters*/_onReturn_);
#endregion NOT Attribute.IsExpired
                }
#endregion NOT IsKeepCallback
#endregion IF IsClientAsynchronous
#endregion NOT IsNullMethod
#endregion LOOP MethodIndexs
            }
#endregion IF Attribute.IsLoadBalancing

#region IF Attribute.IsClientInterface
            /// <summary>
            /// TCP客户端操作接口
            /// </summary>
            public interface ITcpClient : IDisposable
            {
                /// <summary>
                /// 忽略TCP分组
                /// </summary>
                /// <param name="groupId">分组标识</param>
                /// <returns>是否调用成功</returns>
                fastCSharp.net.returnValue.type TcpIgnoreGroup(int groupId);
#region LOOP MethodIndexs
#region NOT IsNullMethod
#region IF MemberIndex
#region IF MethodIsReturn
#region IF Method.XmlDocument
                /// <summary>
                /// @Method.XmlDocument
                /// </summary>
#endregion IF Method.XmlDocument
#region LOOP MethodParameters
#region IF XmlDocument
                /// <param name="@ParameterName">@XmlDocument</param>
#endregion IF XmlDocument
#endregion LOOP MethodParameters
#region IF IsInputParameter
                fastCSharp.net.returnValue<@MethodReturnType.FullName> this[/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterJoinName/*LOOP:MethodParameters*/] { get;/*IF:SetMethod*/ set;/*IF:SetMethod*/ }
#endregion IF IsInputParameter
#region NOT IsInputParameter
                fastCSharp.net.returnValue<@MethodReturnType.FullName> @PropertyName { get;/*IF:SetMethod*/ set;/*IF:SetMethod*/ }
#endregion NOT IsInputParameter
#endregion IF MethodIsReturn
#endregion IF MemberIndex
#region NOT MemberIndex
#region IF ServiceAttribute.IsClientInterface
#region IF IsClientSynchronous
#region IF Method.XmlDocument
                /// <summary>
                /// @Method.XmlDocument
                /// </summary>
#endregion IF Method.XmlDocument
#region LOOP MethodParameters
#region IF XmlDocument
                /// <param name="@ParameterName">@XmlDocument</param>
#endregion IF XmlDocument
#endregion LOOP MethodParameters
#region IF Method.ReturnXmlDocument
                /// <returns>@Method.ReturnXmlDocument</returns>
#endregion IF Method.ReturnXmlDocument
                fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/ /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterJoinName/*LOOP:MethodParameters*/);
#endregion IF IsClientSynchronous
#region IF Attribute.IsClientAsynchronous
#region IF Method.XmlDocument
                /// <summary>
                /// @Method.XmlDocument
                /// </summary>
#endregion IF Method.XmlDocument
#region LOOP MethodParameters
#region IF XmlDocument
                /// <param name="@ParameterName">@XmlDocument</param>
#endregion IF XmlDocument
#endregion LOOP MethodParameters
#region IF Method.ReturnXmlDocument
                /// <param name="_onReturn_">@Method.ReturnXmlDocument</param>
#endregion IF Method.ReturnXmlDocument
#region IF IsKeepCallback
                /// <returns>保持异步回调</returns>
#endregion IF IsKeepCallback
                @KeepCallbackType /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/> _onReturn_);
#endregion IF Attribute.IsClientAsynchronous
#endregion IF ServiceAttribute.IsClientInterface
#endregion NOT MemberIndex
#endregion NOT IsNullMethod
#endregion LOOP MethodIndexs
            }
#endregion IF Attribute.IsClientInterface
            /// <summary>
            /// TCP客户端
            /// </summary>
            public class tcpClient : /*IF:Attribute.IsClientInterface*/ITcpClient, /*IF:Attribute.IsClientInterface*//*IF:Attribute.ClientInterfaceType*/@ClientInterfaceType, /*IF:Attribute.ClientInterfaceType*//*IF:IsTimeVerify*/fastCSharp.net.tcp.timeVerifyServer.ITimeVerifyClient, /*IF:IsTimeVerify*/fastCSharp.net.tcp.commandClient.IClient
            {
#region IF IsTimeVerify
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
#endregion IF IsTimeVerify
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
#region IF IsVerifyMethod
                /// <param name="verifyMethod">TCP验证方法</param>
#endregion IF IsVerifyMethod
#region NOT IsVerifyMethod
                /// <param name="verify">TCP验证实例</param>
#endregion NOT IsVerifyMethod
#if NOJIT
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null/*IF:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethodAsObject verifyMethod = null/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null/*NOT:IsVerifyMethod*/)
#else
                public tcpClient(fastCSharp.code.cSharp.tcpServer attribute = null/*IF:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<tcpClient> verifyMethod = null/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpClientVerify verify = null/*NOT:IsVerifyMethod*/)
#endif
                {
#region IF IsServerCode
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig("@Attribute.ServiceName", typeof(@type.FullName)), @MaxCommandLength/*IF:IsVerifyMethod*/, verifyMethod/*IF:Attribute.VerifyMethodType*/ ?? new @TcpVerifyMethodType()/*IF:Attribute.VerifyMethodType*/, this/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, verify/*IF:Attribute.VerifyType*/ ?? new @TcpVerifyType()/*IF:Attribute.VerifyType*//*NOT:IsVerifyMethod*/);
#endregion IF IsServerCode
#region NOT IsServerCode
                    tcpCommandClient = new fastCSharp.net.tcp.commandClient<tcpClient>(attribute ?? fastCSharp.code.cSharp.tcpServer.GetConfig(TcpServerAttribute, "@Attribute.ServiceName"), @MaxCommandLength/*IF:IsVerifyMethod*/, verifyMethod/*IF:Attribute.VerifyMethodType*/ ?? new @TcpVerifyMethodType()/*IF:Attribute.VerifyMethodType*/, this/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*/, verify/*IF:Attribute.VerifyType*/ ?? new @TcpVerifyType()/*IF:Attribute.VerifyType*//*NOT:IsVerifyMethod*/);
#endregion NOT IsServerCode
                }
                /// <summary>
                /// 释放资源
                /// </summary>
                public void Dispose()
                {
                    fastCSharp.pub.Dispose(ref tcpCommandClient);
                }
#region NOTE
                public fastCSharp.net.returnValue<bool> verify(ulong randomPrefix, byte[] md5Data, ref long ticks) { return false; }
#endregion NOTE

#region NOT IsServerCode
                /// <summary>
                /// TCP服务调用配置
                /// </summary>
                public static fastCSharp.code.cSharp.tcpServer TcpServerAttribute
                {
                    get { return fastCSharp.emit.jsonParser.Parse<fastCSharp.code.cSharp.tcpServer>(@"@AttributeJson"); }
                }
#endregion NOT IsServerCode

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

#region LOOP MethodIndexs
#region NOT IsNullMethod
#region IF ServiceAttribute.IsIdentityCommand
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand @MethodIdentityCommand = new fastCSharp.net.tcp.commandClient.identityCommand { Command = @MethodIndex + @CommandStartIndex, MaxInputSize = @InputParameterMaxLength, IsKeepCallback = @IsKeepCallback, IsSendOnly = @IsClientSendOnly };
#endregion IF ServiceAttribute.IsIdentityCommand
#region NOT ServiceAttribute.IsIdentityCommand
                private static readonly fastCSharp.net.tcp.commandClient.dataCommand @MethodDataCommand = new fastCSharp.net.tcp.commandClient.dataCommand { Command = fastCSharp.net.tcp.commandServer.GetMethodKeyNameCommand("@Method.MethodKeyName"), MaxInputSize = @InputParameterMaxLength, IsKeepCallback = @IsKeepCallback, IsSendOnly = @IsClientSendOnly };
#endregion NOT ServiceAttribute.IsIdentityCommand

#region NOT MemberIndex
#region IF IsClientSynchronous
#region IF Method.XmlDocument
                /// <summary>
                /// @Method.XmlDocument
                /// </summary>
#endregion IF Method.XmlDocument
#region LOOP MethodParameters
#region IF XmlDocument
                /// <param name="@ParameterName">@XmlDocument</param>
#endregion IF XmlDocument
#endregion LOOP MethodParameters
#region IF Method.ReturnXmlDocument
                /// <returns>@Method.ReturnXmlDocument</returns>
#endregion IF Method.ReturnXmlDocument
                public fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/ /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterJoinName/*LOOP:MethodParameters*/)
                {
#region NAME SynchronousMethod
                    fastCSharp.net.returnValue.type _returnType_;
#region IF Attribute.IsExpired
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;
#endregion IF Attribute.IsExpired
#region NOT Attribute.IsExpired
                    fastCSharp.net.waitCall/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _wait_ = fastCSharp.net.waitCall/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/.Get();
                    if (_wait_ == null) _returnType_ = fastCSharp.net.returnValue.type.ClientException;
                    else
                    {
                        /*PUSH:Method*/
                        this.@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterRefName, /*LOOP:MethodParameters*/null, _wait_, false);
#region IF IsOutputParameter
                        fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
#region IF IsOutputParameter
#region LOOP MethodParameters
#region IF IsRefOrOut
                            @ParameterName = /*IF:Method.Method.IsGenericMethod*/(@ParameterType.FullName)/*IF:Method.Method.IsGenericMethod*/_outputParameter_.Value.@ParameterName;
#endregion IF IsRefOrOut
#endregion LOOP MethodParameters
#endregion IF IsOutputParameter
#region IF MethodIsReturn
#region IF IsGenericReturn
                            return (@MethodReturnType.FullName)_outputParameter_.Value.Return;
#endregion IF IsGenericReturn
#region NOT IsGenericReturn
                            return _outputParameter_.Value.Return;
#endregion NOT IsGenericReturn
#endregion IF MethodIsReturn
#region NOT MethodIsReturn
                            return new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = fastCSharp.net.returnValue.type.Success };
#endregion NOT MethodIsReturn
                        }
                        _returnType_ = _outputParameter_.Type;
#endregion IF IsOutputParameter
#region NOT IsOutputParameter
#region IF MemberIndex
                        if ((_returnType_ = _wait_.Wait()) == fastCSharp.net.returnValue.type.Success) return/*NOTE*/ null/*NOTE*/;
#endregion IF MemberIndex
#region NOT MemberIndex
                        fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return /*NOTE*/(fastCSharp.net.returnValue<MethodReturnType.FullName>)(object)/*NOTE*/_returnValue_;
#endregion NOT MemberIndex
#endregion NOT IsOutputParameter
                    }
#endregion NOT Attribute.IsExpired
#region IF IsOutputParameter
#region LOOP MethodParameters
#region IF IsOut
                    @ParameterName = default(@ParameterType.FullName);
#endregion IF IsOut
#endregion LOOP MethodParameters
#endregion IF IsOutputParameter
#endregion NAME SynchronousMethod
                    return new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = _returnType_ };
                }
#endregion IF IsClientSynchronous
#region IF IsClientAsynchronous
#region IF Method.XmlDocument
                /// <summary>
                /// @Method.XmlDocument
                /// </summary>
#endregion IF Method.XmlDocument
#region LOOP MethodParameters
#region IF XmlDocument
                /// <param name="@ParameterName">@XmlDocument</param>
#endregion IF XmlDocument
#endregion LOOP MethodParameters
#region IF Method.ReturnXmlDocument
                /// <param name="_onReturn_">@Method.ReturnXmlDocument</param>
#endregion IF Method.ReturnXmlDocument
#region IF IsKeepCallback
                /// <returns>保持异步回调</returns>
#endregion IF IsKeepCallback
                public @KeepCallbackType /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/> _onReturn_)
                {
#region IF Attribute.IsExpired
                    _onReturn_(new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = fastCSharp.net.returnValue.type.VersionExpired });
#endregion IF Attribute.IsExpired
#region IF IsOutputParameter
#region NOT Attribute.IsExpired
                    fastCSharp.net.callback<fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/> _onOutput_;
#region IF IsGenericReturn
                    _onOutput_ = /*NOTE*/(fastCSharp.net.callback<fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/>)(object)/*NOTE*/fastCSharp.net.asynchronousMethod.callReturnGeneric</*IF:MethodIsReturn*/@MethodReturnType.FullName, /*IF:MethodIsReturn*//*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterGenericTypeName>.Get(_onReturn_);
#endregion IF IsGenericReturn
#region NOT IsGenericReturn
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn</*IF:MethodIsReturn*/@MethodReturnType.FullName, /*IF:MethodIsReturn*/tcpServer.@OutputParameterTypeName>.Get(_onReturn_);
#endregion NOT IsGenericReturn
                    if (_onReturn_ == null || _onOutput_ != null)
                    {
                        /*IF:IsKeepCallback*/
                        return /*IF:IsKeepCallback*//*NOTE*/(KeepCallbackType)(object)/*NOTE*//*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterRefName, /*LOOP:MethodParameters*/null, _onOutput_/*PUSH:Attribute*/, @IsClientCallbackTask/*PUSH:Attribute*/);
                    }
#region LOOP MethodParameters
#region IF IsOut
                    @ParameterName = /*NOTE*/(ParameterTypeRefName)(object)/*NOTE*/default(@ParameterType.FullName);
#endregion IF IsOut
#endregion LOOP MethodParameters
#region IF IsKeepCallback
                    return null;
#endregion IF IsKeepCallback
#endregion NOT Attribute.IsExpired
#endregion IF IsOutputParameter
#region NOT IsOutputParameter
#region NOT Attribute.IsExpired
                    /*IF:IsKeepCallback*/
                    return /*IF:IsKeepCallback*//*PUSH:Method*/this.@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterName, /*LOOP:MethodParameters*//*NOTE*/(Action<fastCSharp.net.returnValue>)(object)/*NOTE*/_onReturn_, null/*PUSH:Attribute*/, @IsClientCallbackTask/*PUSH:Attribute*/);
#endregion NOT Attribute.IsExpired
#endregion NOT IsOutputParameter
                }
#endregion IF IsClientAsynchronous
#endregion NOT MemberIndex

#region IF MemberIndex
#region IF MethodIsReturn
#region IF Method.XmlDocument
                /// <summary>
                /// @Method.XmlDocument
                /// </summary>
#endregion IF Method.XmlDocument
#region LOOP MethodParameters
#region IF XmlDocument
                /// <param name="@ParameterName">@XmlDocument</param>
#endregion IF XmlDocument
#endregion LOOP MethodParameters
#region IF IsInputParameter
                public fastCSharp.net.returnValue<@MethodReturnType.FullName> this[/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterJoinName/*LOOP:MethodParameters*/]
                {
                    get
                    {
#region NOTE
                        fastCSharp.net.returnValue.type _returnType_ = fastCSharp.net.returnValue.type.Unknown;
#endregion NOTE
#region FROMNAME SynchronousMethod
#endregion FROMNAME SynchronousMethod
                        return new fastCSharp.net.returnValue<@MethodReturnType.FullName> { Type = _returnType_ };
                    }
#region PUSH SetMethod
                    set
                    {
#region FROMNAME SynchronousMethod
#endregion FROMNAME SynchronousMethod
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }
#endregion PUSH SetMethod
                }
#endregion IF IsInputParameter
#region NOT IsInputParameter
                public fastCSharp.net.returnValue<@MethodReturnType.FullName> @PropertyName
                {
                    get
                    {
#region NOTE
                        fastCSharp.net.returnValue.type _returnType_ = fastCSharp.net.returnValue.type.Unknown;
#endregion NOTE
#region FROMNAME SynchronousMethod
#endregion FROMNAME SynchronousMethod
                        return new fastCSharp.net.returnValue<@MethodReturnType.FullName> { Type = _returnType_ };
                    }
#region PUSH SetMethod
                    set
                    {
#region FROMNAME SynchronousMethod
#endregion FROMNAME SynchronousMethod
                        fastCSharp.log.Error.Throw(fastCSharp.log.exceptionType.ErrorOperation);
                    }
#endregion PUSH SetMethod
                }
#endregion NOT IsInputParameter
#endregion IF MethodIsReturn
#endregion IF MemberIndex

#region IF IsOutputParameter
                private @KeepCallbackType /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _returnType_ = new fastCSharp.net.returnValue/*IF:IsOutputParameter*/<tcpServer.@OutputParameterTypeName>/*IF:IsOutputParameter*/();
#region LOOP MethodParameters
#region IF IsOut
                    @ParameterName = /*NOTE*/(ParameterTypeRefName)(object)/*NOTE*/default(@ParameterType.FullName);
#endregion IF IsOut
#endregion LOOP MethodParameters
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = /*NOT:IsVerifyMethod*/TcpCommandClient.StreamSocket/*NOT:IsVerifyMethod*//*NOTE*/ ?? /*NOTE*//*IF:IsVerifyMethod*/TcpCommandClient.VerifyStreamSocket/*IF:IsVerifyMethod*/;
                        if (_socket_ != null)
                        {
#region IF IsInputParameter
                            tcpServer.@InputParameterTypeName _inputParameter_ = new tcpServer.@InputParameterTypeName
                            {
#region IF Method.Method.IsGenericMethod
                                @GenericParameterTypeName = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0/*LOOP:Method.GenericParameters*/, typeof(@FullName)/*LOOP:Method.GenericParameters*/),
#endregion IF Method.Method.IsGenericMethod
#region IF IsGenericParameterCallback
                                @ReturnTypeName = typeof(@MethodReturnType.FullName),
#endregion IF IsGenericParameterCallback
#region LOOP MethodParameters
#region NOT IsOut
                                @ParameterName = /*IF:ParameterType.IsStream*//*NOTE*/(FullName)(object)/*NOTE*/TcpCommandClient.GetTcpStream(/*NOTE*/(System.IO.Stream)(object)/*NOTE*//*IF:ParameterType.IsStream*/@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/,
#endregion NOT IsOut
#endregion LOOP MethodParameters
                            };
#endregion IF IsInputParameter
#region LOOP MethodParameters
#region IF IsRef
                            _returnType_.Value.@ParameterName = _inputParameter_.@ParameterName;
#endregion IF IsRef
#endregion LOOP MethodParameters
#region IF ReturnInputParameterName
                            _returnType_.Value.Ret = _inputParameter_.@ReturnInputParameterName;
#endregion IF ReturnInputParameterName
#region IF ServiceAttribute.IsIdentityCommand
                            /*IF:IsKeepCallback*/
                            return /*IF:IsKeepCallback*//*NOTE*/(KeepCallbackType)(object)/*NOTE*/_socket_.Get/*AT:JsonCall*/(_onReturn_, _callback_, @MethodIdentityCommand/*IF:IsInputParameter*/, ref _inputParameter_/*IF:IsInputParameter*/, ref _returnType_.Value, _isTask_);
#endregion IF ServiceAttribute.IsIdentityCommand
#region NOT ServiceAttribute.IsIdentityCommand
                            /*IF:IsKeepCallback*/
                            return /*IF:IsKeepCallback*//*NOTE*/(KeepCallbackType)(object)/*NOTE*/_socket_.Get/*AT:JsonCall*/(_onReturn_, _callback_, @MethodDataCommand/*IF:IsInputParameter*/, ref _inputParameter_/*IF:IsInputParameter*/, ref _returnType_.Value, _isTask_);
#endregion NOT ServiceAttribute.IsIdentityCommand
#region NOT IsKeepCallback
                            return/*NOTE*/ null/*NOTE*/;
#endregion NOT IsKeepCallback
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
#region IF IsKeepCallback
                    return null;
#endregion IF IsKeepCallback
                }
#endregion IF IsOutputParameter
#region NOT IsOutputParameter
#region NOT IsKeepCallback
#region IF Method.XmlDocument
                /// <summary>
                /// @Method.XmlDocument
                /// </summary>
#endregion IF Method.XmlDocument
#region LOOP MethodParameters
#region IF XmlDocument
                /// <param name="@ParameterName">@XmlDocument</param>
#endregion IF XmlDocument
#endregion LOOP MethodParameters
#region IF Method.ReturnXmlDocument
                /// <param name="_onReturn_">@Method.ReturnXmlDocument</param>
#endregion IF Method.ReturnXmlDocument
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public void /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue> _onReturn_)
                {
#region IF Attribute.IsExpired
                    _onReturn_(new fastCSharp.net.returnValue { Type = fastCSharp.net.returnValue.type.VersionExpired });
#endregion IF Attribute.IsExpired
#region NOT Attribute.IsExpired
                    /*PUSH:Method*/
                    @MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterRefName, /*LOOP:MethodParameters*/_onReturn_, null/*PUSH:Attribute*/, @IsClientCallbackTask/*PUSH:Attribute*/);
#endregion NOT Attribute.IsExpired
                }
#endregion NOT IsKeepCallback
                private @KeepCallbackType /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = /*NOT:IsVerifyMethod*/TcpCommandClient.StreamSocket/*NOT:IsVerifyMethod*//*NOTE*/ ?? /*NOTE*//*IF:IsVerifyMethod*/TcpCommandClient.VerifyStreamSocket/*IF:IsVerifyMethod*/;
                        if (_socket_ != null)
                        {
#region IF IsInputParameter
                            tcpServer.@InputParameterTypeName _inputParameter_ = new tcpServer.@InputParameterTypeName
                            {
#region IF Method.Method.IsGenericMethod
                                @GenericParameterTypeName = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0/*LOOP:Method.GenericParameters*/, typeof(@FullName)/*LOOP:Method.GenericParameters*/),
#endregion IF Method.Method.IsGenericMethod
#region IF IsGenericParameterCallback
                                @ReturnTypeName = typeof(@MethodReturnType.FullName),
#endregion IF IsGenericParameterCallback
#region LOOP MethodParameters
#region NOT IsOut
                                @ParameterName = /*IF:ParameterType.IsStream*//*NOTE*/(FullName)(object)/*NOTE*/TcpCommandClient.GetTcpStream(/*NOTE*/(System.IO.Stream)(object)/*NOTE*//*IF:ParameterType.IsStream*/@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/,
#endregion NOT IsOut
#endregion LOOP MethodParameters
                            };
#endregion IF IsInputParameter
#region IF ServiceAttribute.IsIdentityCommand
                            /*IF:IsKeepCallback*/
                            return /*IF:IsKeepCallback*//*NOTE*/(KeepCallbackType)(object)/*NOTE*/_socket_.Call/*IF:IsInputParameter*//*AT:JsonCall*/<tcpServer.@InputParameterTypeName>/*IF:IsInputParameter*/(_onReturn_, _callback_, @MethodIdentityCommand/*IF:IsInputParameter*/, ref _inputParameter_/*IF:IsInputParameter*/, _isTask_);
#endregion IF ServiceAttribute.IsIdentityCommand
#region NOT ServiceAttribute.IsIdentityCommand
                            /*IF:IsKeepCallback*/
                            return /*IF:IsKeepCallback*//*NOTE*/(KeepCallbackType)(object)/*NOTE*/_socket_.Call/*IF:IsInputParameter*//*AT:JsonCall*/<tcpServer.@InputParameterTypeName>/*IF:IsInputParameter*/(_onReturn_, _callback_, @MethodDataCommand/*IF:IsInputParameter*/, ref _inputParameter_/*IF:IsInputParameter*/, _isTask_);
#endregion NOT ServiceAttribute.IsIdentityCommand
#region NOT IsKeepCallback
                            return/*NOTE*/ null/*NOTE*/;
#endregion NOT IsKeepCallback
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
#region IF IsKeepCallback
                    return null;
#endregion IF IsKeepCallback
                }
#endregion NOT IsOutputParameter
#endregion NOT IsNullMethod
#endregion LOOP MethodIndexs
            }
#endregion IF IsClientCode
#endregion NOT IsRemember

#region PART REMEMBER
#region IF IsRemember
            /// <summary>
            /// TCP服务命令序号记忆数据
            /// </summary>
            static class @ServiceNameOnly
            {
                /// <summary>
                /// 命令序号记忆数据
                /// </summary>
                private static fastCSharp.keyValue<string, int>[] @RememberIdentityCommeandName()
                {
                    fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[@MethodIndexs.Length];
#region LOOP MethodIndexs
#region NOT IsNullMethod
                    names[@MethodIndex].Set(@"@Method.MethodKeyName", @MethodIndex);
#endregion NOT IsNullMethod
#endregion LOOP MethodIndexs
                    return names;
                }
            }
#endregion IF IsRemember
#endregion PART REMEMBER
        }
#endregion PART CLASS
    }
#region NOTE
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 保持异步回调类型
        /// </summary>
        public class KeepCallbackType { }
        /// <summary>
        /// 类型全名
        /// </summary>
        public partial class FullName : fastCSharp.code.cSharp.tcpServer.ICommandServer
        {
            /// <summary>
            /// 设置TCP服务端
            /// </summary>
            /// <param name="tcpServer">TCP服务端</param>
            public void SetCommandServer(fastCSharp.net.tcp.commandServer tcpServer)
            {
            }
        }
        /// <summary>
        /// 客户端附加接口类型
        /// </summary>
        public interface ClientInterfaceType { }
    }
#endregion NOTE
}
