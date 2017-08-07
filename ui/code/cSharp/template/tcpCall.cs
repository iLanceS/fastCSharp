using System;
#pragma warning disable 162

namespace fastCSharp.code.cSharp.template
{
    class TcpCall : pub
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
        private const int GroupId = 0;
        public static fastCSharp.net.returnValue<bool> MethodGenericName(ulong randomPrefix, byte[] md5Data, ref long ticks) { return false; }
        #endregion NOTE

        #region PART CLASS
        #region NOT IsAllType
        #region PART SERVERCALL
        /*NOTE*/
        public partial class /*NOTE*/@TypeNameDefinition
        {
            #region IF type.Type.IsGenericType
            [fastCSharp.code.ignore]
            [fastCSharp.code.cSharp.tcpCall(IsGenericTypeServerMethod = true)]
            #endregion IF type.Type.IsGenericType
            internal static partial class @GenericTypeServerName
            {
                #region LOOP MethodIndexs
                #region NOT IsNullMethod
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static /*PUSH:Method*/@ReturnType.FullName/*PUSH:Method*/ @MethodIndexGenericName(/*LOOP:Method.Parameters*/@ParameterTypeRefName @ParameterJoinName/*LOOP:Method.Parameters*/)
                {
                    #region IF MemberIndex
                    #region NOT Method.IsGetMember
                    @type.FullName/**/.@StaticPropertyName = /*LOOP:Method.Parameters*/@ParameterName/*LOOP:Method.Parameters*/;
                    #endregion NOT Method.IsGetMember
                    #region IF Method.IsGetMember
                    return /*NOTE*/(FullName)/*NOTE*/@type.FullName/**/.@StaticPropertyName;
                    #endregion IF Method.IsGetMember
                    #endregion IF MemberIndex

                    #region NOT MemberIndex
                    #region PUSH Method
                    /*IF:IsReturn*/
                    return /*NOTE*/(FullName)/*NOTE*//*IF:IsReturn*/@type.FullName/**/.@StaticMethodGenericName(/*LOOP:Parameters*/@ParameterJoinRefName/*LOOP:Parameters*/);
                    #endregion PUSH Method
                    #endregion NOT MemberIndex
                }
                #region NOT MethodType.Type.IsGenericType
                #region IF Method.Method.IsGenericMethod
                public static readonly System.Reflection.MethodInfo @GenericMethodInfoName;
                #endregion IF Method.Method.IsGenericMethod
                #endregion NOT MethodType.Type.IsGenericType
                #endregion NOT IsNullMethod
                #endregion LOOP MethodIndexs
                #region NOT type.Type.IsGenericType
                #region IF IsAnyGenericMethod
                static @GenericTypeServerName()
                {
                    System.Collections.Generic.Dictionary<fastCSharp.code.cSharp.tcpBase.genericMethod, System.Reflection.MethodInfo> genericMethods = fastCSharp.code.cSharp.tcpCall.GetGenericMethods(typeof(@type.FullName));
                    #region LOOP MethodIndexs
                    #region NOT IsNullMethod
                    #region IF Method.Method.IsGenericMethod
                    @GenericMethodInfoName = /*PUSH:Method*/genericMethods[new fastCSharp.code.cSharp.tcpBase.genericMethod("@MethodName", @GenericParameters.Length/*LOOP:Parameters*/, "@ParameterRef@ParameterType.FullName"/*LOOP:Parameters*/)]/*PUSH:Method*/;
                    #endregion IF Method.Method.IsGenericMethod
                    #endregion NOT IsNullMethod
                    #endregion LOOP MethodIndexs
                }
                #endregion IF IsAnyGenericMethod
                #endregion NOT type.Type.IsGenericType
            }
        }
        #endregion PART SERVERCALL
        #region PART CLIENTCALL
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public static partial class tcpCall
        {
            #region IF type.XmlDocument
            /// <summary>
            /// @type.XmlDocument
            /// </summary>
            #endregion IF type.XmlDocument
            public /*NOTE*/partial class /*NOTE*/@NoAccessTypeNameDefinition/*IF:IsTimeVerify*/ : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod/*IF:IsTimeVerify*/
            {
                #region LOOP MethodIndexs
                #region NOT IsNullMethod
                #region IF IsVerifyMethod
                #region IF IsTimeVerify
                /// <summary>
                /// TCP调用验证客户端
                /// </summary>
                /// <returns></returns>
                public bool Verify()
                {
                    return fastCSharp.net.tcp.timeVerifyServer.tcpCall<@TcpTimeVerifyMethodType>.Verify(/*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ClientPart/**/.@ServiceName/**/.Default, /*NOTE*/TcpCall./*NOTE*//*PUSH:Method*/@MethodGenericName/*PUSH:Method*/);
                }
                #endregion IF IsTimeVerify
                #endregion IF IsVerifyMethod
                #region IF ServiceAttribute.IsIdentityCommand
                private static readonly fastCSharp.net.tcp.commandClient.identityCommand @MethodIdentityCommand = new fastCSharp.net.tcp.commandClient.identityCommand { Command = @MethodIndex + @CommandStartIndex, MaxInputSize = @InputParameterMaxLength, IsKeepCallback = @IsKeepCallback, IsSendOnly = @IsClientSendOnly };
                #endregion IF ServiceAttribute.IsIdentityCommand
                #region NOT ServiceAttribute.IsIdentityCommand
                private static readonly fastCSharp.net.tcp.commandClient.dataCommand @MethodDataCommand = new fastCSharp.net.tcp.commandClient.dataCommand { Command = fastCSharp.net.tcp.commandServer.GetMethodKeyNameCommand("@Method.MethodKeyFullName"), MaxInputSize = @InputParameterMaxLength, IsKeepCallback = @IsKeepCallback, IsSendOnly = @IsClientSendOnly };
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
                public static fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/ /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterJoinName/*LOOP:MethodParameters*/)
                {
                    #region NAME SynchronousMethod
                    fastCSharp.net.returnValue.type _returnType_;
                    #region IF Attribute.IsExpired
                    _returnType_ = fastCSharp.net.returnValue.type.VersionExpired;
                    #endregion IF Attribute.IsExpired
                    #region NOT Attribute.IsExpired
                    fastCSharp.net.waitCall/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _wait_ = fastCSharp.net.waitCall/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/.Get();
                    if (_wait_ != null)
                    {
                        /*PUSH:Method*/
                        @MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterRefName, /*LOOP:MethodParameters*/null, _wait_, false);
                        #region IF IsOutputParameter
                        fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _outputParameter_;
                        _wait_.Get(out _outputParameter_);
                        if (_outputParameter_.Type == fastCSharp.net.returnValue.type.Success)
                        {
                            #region IF MethodIsReturn

                            /*PUSH:AutoParameter*/
                            @DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName _outputParameterValue_ = _outputParameter_.Value;
                            #region LOOP MethodParameters
                            #region IF IsRefOrOut
                            @ParameterName = /*IF:Method.Method.IsGenericMethod*/(@ParameterType.FullName)/*IF:Method.Method.IsGenericMethod*/_outputParameterValue_.@ParameterName;
                            #endregion IF IsRefOrOut
                            #endregion LOOP MethodParameters
                            #region IF IsGenericReturn
                            return (@MethodReturnType.FullName)_outputParameterValue_.Return;
                            #endregion IF IsGenericReturn
                            #region NOT IsGenericReturn
                            return _outputParameterValue_.Return;
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
                        fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _returnValue_;
                        _wait_.Get(out _returnValue_);
                        return /*NOTE*/(fastCSharp.net.returnValue<MethodReturnType.FullName>)(object)/*NOTE*/_returnValue_;
                        #endregion NOT MemberIndex
                        #endregion NOT IsOutputParameter
                    }
                    else _returnType_ = fastCSharp.net.returnValue.type.ClientException;
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
                public static @KeepCallbackType /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/> _onReturn_)
                {
                    #region IF Attribute.IsExpired
                    _onReturn_(new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = fastCSharp.net.returnValue.type.VersionExpired });
                    #endregion IF Attribute.IsExpired
                    #region IF IsOutputParameter
                    #region NOT Attribute.IsExpired
                    fastCSharp.net.callback<fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/> _onOutput_;
                    #region IF IsGenericReturn
                    _onOutput_ = /*NOTE*/(fastCSharp.net.callback<fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/>)(object)/*NOTE*/fastCSharp.net.asynchronousMethod.callReturnGeneric</*IF:MethodIsReturn*/@MethodReturnType.FullName, /*IF:MethodIsReturn*//*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterGenericTypeName>.Get(_onReturn_);
                    #endregion IF IsGenericReturn
                    #region NOT IsGenericReturn
                    _onOutput_ = fastCSharp.net.asynchronousMethod.callReturn</*IF:MethodIsReturn*/@MethodReturnType.FullName, /*IF:MethodIsReturn*//*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>.Get(_onReturn_);
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
                    #endregion NOT Attribute.IsExpired
                    #region IF IsKeepCallback
                    return null;
                    #endregion IF IsKeepCallback
                    #endregion IF IsOutputParameter
                    #region NOT IsOutputParameter
                    #region NOT Attribute.IsExpired
                    /*IF:IsKeepCallback*/
                    return /*IF:IsKeepCallback*//*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterName, /*LOOP:MethodParameters*//*NOTE*/(Action<fastCSharp.net.returnValue>)(object)/*NOTE*/_onReturn_, null/*PUSH:Attribute*/, @IsClientCallbackTask/*PUSH:Attribute*/);
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
                public static fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/ @PropertyName
                {
                    get
                    {
                        #region NOTE
                        fastCSharp.net.returnValue.type _returnType_ = fastCSharp.net.returnValue.type.Unknown;
                        #endregion NOTE
                        #region FROMNAME SynchronousMethod
                        #endregion FROMNAME SynchronousMethod
                        return new fastCSharp.net.returnValue/*IF:MethodIsReturn*/<@MethodReturnType.FullName>/*IF:MethodIsReturn*/{ Type = _returnType_ };
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
                #endregion IF MethodIsReturn
                #endregion IF MemberIndex

                #region IF IsOutputParameter
                private static @KeepCallbackType /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/ _returnType_ = new fastCSharp.net.returnValue/*IF:IsOutputParameter*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@OutputParameterTypeName>/*IF:IsOutputParameter*/();
                    #region LOOP MethodParameters
                    #region IF IsOut
                    @ParameterName = /*NOTE*/(ParameterTypeRefName)(object)/*NOTE*/default(@ParameterType.FullName);
                    #endregion IF IsOut
                    #endregion LOOP MethodParameters
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = /*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ClientPart/**/.@ServiceName/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = /*NOT:IsVerifyMethod*/_client_.StreamSocket/*NOT:IsVerifyMethod*//*NOTE*/ ?? /*NOTE*//*IF:IsVerifyMethod*/_client_.VerifyStreamSocket/*IF:IsVerifyMethod*/;
                        if (_socket_ != null)
                        {
                            #region IF IsInputParameter
                            /*PUSH:AutoParameter*/
                            @DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@InputParameterTypeName _inputParameter_ = new /*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@InputParameterTypeName
                            {
                                #region IF Method.Method.IsGenericMethod
                                @GenericParameterTypeName = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0/*LOOP:Method.GenericParameters*/, typeof(@FullName)/*LOOP:Method.GenericParameters*/),
                                #endregion IF Method.Method.IsGenericMethod
                                #region IF IsGenericParameterCallback
                                @ReturnTypeName = typeof(@MethodReturnType.FullName),
                                #endregion IF IsGenericParameterCallback
                                #region LOOP MethodParameters
                                #region NOT IsOut
                                @ParameterName = /*IF:ParameterType.IsStream*//*NOTE*/(FullName)(object)/*NOTE*/_client_.GetTcpStream(/*NOTE*/(System.IO.Stream)(object)/*NOTE*//*IF:ParameterType.IsStream*/@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/,
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
                public static void /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue> _onReturn_)
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
                private static @KeepCallbackType /*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*/@ParameterTypeRefName @ParameterName, /*LOOP:MethodParameters*/Action<fastCSharp.net.returnValue> _onReturn_, fastCSharp.net.callback<fastCSharp.net.returnValue> _callback_, bool _isTask_)
                {
                    fastCSharp.net.returnValue _returnType_;
                    try
                    {
                        fastCSharp.net.tcp.commandClient _client_ = /*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ClientPart/**/.@ServiceName/**/.Default;
                        fastCSharp.net.tcp.commandClient.streamCommandSocket _socket_ = /*NOT:IsVerifyMethod*/_client_.StreamSocket/*NOT:IsVerifyMethod*//*NOTE*/ ?? /*NOTE*//*IF:IsVerifyMethod*/_client_.VerifyStreamSocket/*IF:IsVerifyMethod*/;
                        if (_socket_ != null)
                        {
                            #region IF IsInputParameter
                            /*PUSH:AutoParameter*/
                            @DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@InputParameterTypeName _inputParameter_ = new /*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@InputParameterTypeName
                            {
                                #region IF Method.Method.IsGenericMethod
                                @GenericParameterTypeName = fastCSharp.code.cSharp.tcpBase.GetGenericParameters(0/*LOOP:Method.GenericParameters*/, typeof(@FullName)/*LOOP:Method.GenericParameters*/),
                                #endregion IF Method.Method.IsGenericMethod
                                #region IF IsGenericParameterCallback
                                @ReturnTypeName = typeof(@MethodReturnType.FullName),
                                #endregion IF IsGenericParameterCallback
                                #region LOOP MethodParameters
                                #region NOT IsOut
                                @ParameterName = /*IF:ParameterType.IsStream*//*NOTE*/(FullName)(object)/*NOTE*/_client_.GetTcpStream(/*NOTE*/(System.IO.Stream)(object)/*NOTE*//*IF:ParameterType.IsStream*/@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/,
                                #endregion NOT IsOut
                                #endregion LOOP MethodParameters
                            };
                            #endregion IF IsInputParameter
                            #region IF ServiceAttribute.IsIdentityCommand
                            /*IF:IsKeepCallback*/
                            return /*IF:IsKeepCallback*//*NOTE*/(KeepCallbackType)(object)/*NOTE*/_socket_.Call/*IF:IsInputParameter*//*AT:JsonCall*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@InputParameterTypeName>/*IF:IsInputParameter*/(_onReturn_, _callback_, @MethodIdentityCommand/*IF:IsInputParameter*/, ref _inputParameter_/*IF:IsInputParameter*/, _isTask_);
                            #endregion IF ServiceAttribute.IsIdentityCommand
                            #region NOT ServiceAttribute.IsIdentityCommand
                            /*IF:IsKeepCallback*/
                            return /*IF:IsKeepCallback*//*NOTE*/(KeepCallbackType)(object)/*NOTE*/_socket_.Call/*IF:IsInputParameter*//*AT:JsonCall*/</*PUSH:AutoParameter*/@DefaultNamespace/*PUSH:AutoParameter*/.@ParameterPart/**/.@ServiceName/**/.@InputParameterTypeName>/*IF:IsInputParameter*/(_onReturn_, _callback_, @MethodDataCommand/*IF:IsInputParameter*/, ref _inputParameter_/*IF:IsInputParameter*/, _isTask_);
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
        }
        #endregion PART CLIENTCALL
        #endregion NOT IsAllType

        #region IF IsAllType
        #region PART SERVER
        /// <summary>
        /// TCP调用服务端
        /// </summary>
        public partial class @ServiceName : fastCSharp.net.tcp.commandServer
        {
            /// <summary>
            /// TCP调用服务端
            /// </summary>
            /// <param name="attribute">TCP调用服务器端配置信息</param>
            /// <param name="verify">TCP验证实例</param>
            public @ServiceName(fastCSharp.code.cSharp.tcpServer attribute = null/*NOT:IsVerifyMethod*/, fastCSharp.code.cSharp.tcpBase.ITcpVerify verify = null/*NOT:IsVerifyMethod*/)
                : base(attribute ?? fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("@ServiceName"/*IF:TcpServerAttributeType*/, typeof(@TcpServerAttributeType)/*IF:TcpServerAttributeType*/)/*NOT:IsVerifyMethod*/, verify/*IF:ServiceAttribute.VerifyType*/ ?? new @TcpVerifyType()/*IF:ServiceAttribute.VerifyType*//*NOT:IsVerifyMethod*/)
            {
                #region IF ServiceAttribute.IsIdentityCommand
                setCommands(@MethodIndexs.Length);
                #region LOOP MethodIndexs
                #region NOT IsNullMethod
                identityOnCommands[/*IF:IsVerifyMethod*/verifyCommandIdentity = /*IF:IsVerifyMethod*/@MethodIndex + @CommandStartIndex].Set(@MethodIndex + @CommandStartIndex/*IF:IsInputParameterMaxLength*/, @InputParameterMaxLength/*IF:IsInputParameterMaxLength*/);
                #endregion NOT IsNullMethod
                #endregion LOOP MethodIndexs
                #endregion IF ServiceAttribute.IsIdentityCommand
                #region NOT ServiceAttribute.IsIdentityCommand
                int commandIndex;
                keyValue<byte[][], command[]> onCommands = getCommands(@MethodIndexs.Length, out commandIndex);
                #region LOOP MethodIndexs
                #region IF IsNullMethod
                onCommands.Key[commandIndex] = formatMethodKeyName("-@MethodIndexName");
                onCommands.Value[commandIndex++] = default(command);
                #endregion IF IsNullMethod
                #region NOT IsNullMethod
                onCommands.Key[commandIndex] = /*IF:IsVerifyMethod*/verifyCommand = /*IF:IsVerifyMethod*/formatMethodKeyName("@Method.MethodKeyFullName");
                onCommands.Value[commandIndex++].Set(@MethodIndex + @CommandStartIndex/*IF:IsInputParameterMaxLength*/, @InputParameterMaxLength/*IF:IsInputParameterMaxLength*/);
                #endregion NOT IsNullMethod
                #endregion LOOP MethodIndexs
                maxCommandLength = @MaxCommandLength;
                this.onCommands = new fastCSharp.stateSearcher.ascii<command>(onCommands.Key, onCommands.Value);
                #endregion NOT ServiceAttribute.IsIdentityCommand
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
            /// <summary>
            /// 忽略分组
            /// </summary>
            /// <param name="groupId">分组标识</param>
            protected override void ignoreGroup(int groupId)
            {
                #region IF MethodGroups.Length
                switch (groupId)
                {
                    #region LOOP MethodGroups
                    case @GroupId:
                        @GroupIgnoreName = 1;
                        while (@GroupCountName != 0) System.Threading.Thread.Sleep(1);
                        break;
                    #endregion LOOP MethodGroups
                }
                #endregion IF MethodGroups.Length
            }

            #region LOOP MethodIndexs
            #region NOT IsNullMethod
            #region NAME Parameter
            #region IF IsInputParameter
            [fastCSharp.emit.dataSerialize(IsMemberMap = false/*NOT:Attribute.IsInputSerializeReferenceMember*/, IsReferenceMember = false/*NOT:Attribute.IsInputSerializeReferenceMember*/)]
            #region IF Attribute.IsInputSerializeBox
            [fastCSharp.emit.boxSerialize]
            #endregion IF Attribute.IsInputSerializeBox
            internal /*AT:InputParameterClassType*//*NOTE*/partial class/*NOTE*/ @InputParameterTypeName
            {
                #region IF MethodType.Type.IsGenericType
                public fastCSharp.code.remoteType @TypeGenericParameterName;
                #endregion IF MethodType.Type.IsGenericType
                #region IF Method.Method.IsGenericMethod
                public fastCSharp.code.remoteType[] @GenericParameterTypeName;
                #endregion IF Method.Method.IsGenericMethod
                #region IF IsGenericParameterCallback
                public fastCSharp.code.remoteType @ReturnTypeName;
                #endregion IF IsGenericParameterCallback
                #region LOOP MethodParameters
                #region IF ParameterType.IsStream
                public fastCSharp.code.cSharp.tcpBase.tcpStream @StreamParameterName/*NOTE*/ = default(fastCSharp.code.cSharp.tcpBase.tcpStream)/*NOTE*/;
                #endregion IF ParameterType.IsStream
                #region NOT ParameterType.IsStream
                public @GenericParameterType.FullName @ParameterName;
                #endregion NOT ParameterType.IsStream
                #endregion LOOP MethodParameters
                #region NOTE
                public object ParameterJoinName = null;
                public object ParameterRealName = null;
                public GenericParameterType.FullName ReturnInputParameterName = null;
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
            #endregion NAME Parameter
            #region NOTE
            internal class @OutputParameterGenericTypeName : fastCSharp.net.asynchronousMethod.IReturnParameter<object>
            {
                public object Return  { get; set; }
            }
            #endregion NOTE
            #region NOT IsAsynchronousCallback
            #region IF IsMethodServerCall
            sealed class @MethodStreamName : fastCSharp.net.tcp.commandServer.socketCall<@MethodStreamName/*IF:IsInputParameter*/, @InputParameterTypeName/*IF:IsInputParameter*/>
            {
                private void get(ref fastCSharp.net.returnValue/*IF:IsOutputParameter*/<@OutputParameterTypeName>/*IF:IsOutputParameter*/ value)
                {
                    try
                    {
                        #region IF IsInputParameter
                        #region IF IsInvokeGenericMethod
                        object[] invokeParameter = new object[] { /*IF:ClientParameterName*/socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/};
                        #endregion IF IsInvokeGenericMethod
                        #endregion IF IsInputParameter
                        /*IF:MethodIsReturn*/
                        @GenericReturnType.FullName @ReturnName;/*IF:MethodIsReturn*/

                        #region IF MemberIndex
                        #region IF Method.IsGetMember
                        @ReturnName = /*NOTE*/(GenericReturnType.FullName)/*NOTE*/@MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName();
                        #endregion IF Method.IsGetMember
                        #region NOT Method.IsGetMember
                        @MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*LOOP:MethodParameters*/inputParameter.@ParameterName/*LOOP:MethodParameters*/);
                        #endregion NOT Method.IsGetMember
                        #endregion IF MemberIndex

                        #region NOT MemberIndex
                        #region IF MethodType.Type.IsGenericType
                        /*IF:MethodIsReturn*/
                        @ReturnName = (@GenericReturnType.FullName)/*IF:MethodIsReturn*/fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.@TypeGenericParameterName, "@MethodIndexName"/*IF:Method.Method.IsGenericMethod*/, inputParameter.@GenericParameterTypeName/*IF:Method.Method.IsGenericMethod*//*IF:IsInputParameter*/, invokeParameter/*IF:IsInputParameter*/);
                        #endregion IF MethodType.Type.IsGenericType
                        #region NOT MethodType.Type.IsGenericType
                        #region IF Method.Method.IsGenericMethod
                        /*IF:MethodIsReturn*/
                        @ReturnName = (@GenericReturnType.FullName)/*IF:MethodIsReturn*/fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(@MethodType.FullName/**/.@GenericTypeServerName/**/.@GenericMethodInfoName, inputParameter.@GenericParameterTypeName/*IF:IsInputParameter*/, invokeParameter/*IF:IsInputParameter*/);
                        #endregion IF Method.Method.IsGenericMethod
                        #region NOT Method.Method.IsGenericMethod
                        /*IF:MethodIsReturn*/
                        @ReturnName = /*IF:MethodIsReturn*//*NOTE*/(GenericReturnType.FullName)/*NOTE*/@MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*IF:ClientParameterName*/socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/);
                        #endregion NOT Method.Method.IsGenericMethod
                        #endregion NOT MethodType.Type.IsGenericType
                        #endregion NOT MemberIndex

                        #region IF IsOutputParameter
                        #region IF IsInvokeGenericMethod
                        #region LOOP MethodParameters
                        #region IF IsRefOrOut
                        inputParameter.@ParameterName = (@GenericParameterType.FullName)invokeParameter[@ParameterIndex];
                        #endregion IF IsRefOrOut
                        #endregion LOOP MethodParameters
                        #endregion IF IsInvokeGenericMethod
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
                        #region IF IsInvokeGenericMethod
                        object[] invokeParameter = new object[] { /*IF:ClientParameterName*/Socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/Socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*/InputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/};
                        #endregion IF IsInvokeGenericMethod
                        #endregion IF IsInputParameter
                        /*IF:MethodIsReturn*/
                        @GenericReturnType.FullName @ReturnName = /*IF:MethodIsReturn*/
                        #region IF MethodType.Type.IsGenericType
                            /*IF:MethodIsReturn*/(@GenericReturnType.FullName)/*IF:MethodIsReturn*/fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref InputParameter.@TypeGenericParameterName, "@MethodIndexName"/*IF:Method.Method.IsGenericMethod*/, InputParameter.@GenericParameterTypeName/*IF:Method.Method.IsGenericMethod*//*IF:IsInputParameter*/, invokeParameter/*IF:IsInputParameter*/);
                        #endregion IF MethodType.Type.IsGenericType
                        #region NOT MethodType.Type.IsGenericType
                        #region NOTE
                        object _ =
                        #endregion NOTE
                        #region IF Method.Method.IsGenericMethod
                            /*IF:MethodIsReturn*/(@GenericReturnType.FullName)/*IF:MethodIsReturn*/fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(@MethodType.FullName/**/.@GenericTypeServerName/**/.@GenericMethodInfoName, InputParameter.@GenericParameterTypeName/*IF:IsInputParameter*/, invokeParameter/*IF:IsInputParameter*/);
                        #endregion IF Method.Method.IsGenericMethod
                        #region NOT Method.Method.IsGenericMethod
                        @MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*IF:ClientParameterName*/Socket/*IF:MethodParameters.Length*/, /*IF:MethodParameters.Length*//*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/Socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*/InputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*AT:ParameterJoin*//*LOOP:MethodParameters*/);
                        #endregion NOT Method.Method.IsGenericMethod
                        #endregion NOT MethodType.Type.IsGenericType
                        #region IF IsOutputParameter
                        #region IF IsInvokeGenericMethod
                        #region LOOP MethodParameters
                        #region IF IsRefOrOut
                        InputParameter.@ParameterName = (@GenericParameterType.FullName)invokeParameter[@ParameterIndex];
                        #endregion IF IsRefOrOut
                        #endregion LOOP MethodParameters
                        #endregion IF IsInvokeGenericMethod
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
                        returnType = fastCSharp.net.returnValue.type.ClientException;
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
                                #region IF MethodType.Type.IsGenericType
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.@TypeGenericParameterName, "@MethodIndexName"/*IF:Method.Method.IsGenericMethod*/, inputParameter.@GenericParameterTypeName/*IF:Method.Method.IsGenericMethod*/, invokeParameter);
                                #endregion IF MethodType.Type.IsGenericType
                                #region NOT MethodType.Type.IsGenericType
                                #region IF Method.Method.IsGenericMethod
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(@MethodType.FullName/**/.@GenericTypeServerName/**/.@GenericMethodInfoName, inputParameter.@GenericParameterTypeName, invokeParameter);
                                #endregion IF Method.Method.IsGenericMethod
                                #region NOT Method.Method.IsGenericMethod
                                @MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*IF:ClientParameterName*/socket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue<MethodReturnType.FullName>, bool>)/*NOTE*/callbackReturn);
                                #endregion NOT Method.Method.IsGenericMethod
                                #endregion NOT MethodType.Type.IsGenericType
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
                                #region IF MethodType.Type.IsGenericType
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.@TypeGenericParameterName, "@MethodIndexName"/*IF:Method.Method.IsGenericMethod*/, inputParameter.@GenericParameterTypeName/*IF:Method.Method.IsGenericMethod*/, invokeParameter);
                                #endregion IF MethodType.Type.IsGenericType
                                #region NOT MethodType.Type.IsGenericType
                                #region IF Method.Method.IsGenericMethod
                                fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(@MethodType.FullName/**/.@GenericTypeServerName/**/.@GenericMethodInfoName, inputParameter.@GenericParameterTypeName, invokeParameter);
                                #endregion IF Method.Method.IsGenericMethod
                                #region NOT Method.Method.IsGenericMethod
                                @MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*IF:ClientParameterName*/socket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*/, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue, bool>)/*NOTE*/callback);
                                #endregion NOT Method.Method.IsGenericMethod
                                #endregion NOT MethodType.Type.IsGenericType
                            }
                            #endregion NOT MethodIsReturn
                            #endregion IF IsAsynchronousCallback

                            #region NOT IsAsynchronousCallback
                            #region IF IsMethodServerCall
                            #region IF IsServerAsynchronousTask
                            fastCSharp.threading.task.Tiny.Add(@MethodStreamName/**/.GetCall(socket/*IF:IsInputParameter*/, ref inputParameter/*IF:IsInputParameter*/));
                            #endregion IF IsServerAsynchronousTask
                            #region NOT IsServerAsynchronousTask
                            @MethodStreamName/**/.Call(socket/*IF:IsInputParameter*/, ref inputParameter/*IF:IsInputParameter*/);
                            #endregion NOT IsServerAsynchronousTask
                            #endregion IF IsMethodServerCall
                            #region NOT IsMethodServerCall
                            @MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*IF:ClientParameterName*/socket/*IF:ClientParameterName*//*LOOP:MethodParameters*/, /*IF:ParameterType.IsStream*/socket.GetTcpStream(/*AT:Ref*//*NOTE*/(fastCSharp.code.cSharp.tcpBase.tcpStream)(object)/*NOTE*//*IF:ParameterType.IsStream*//*AT:ParameterRef*/inputParameter.@ParameterName/*IF:ParameterType.IsStream*/)/*IF:ParameterType.IsStream*//*LOOP:MethodParameters*/);
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
                            @OutputParameterTypeName ouputParameter = new @OutputParameterTypeName();
                            invokeParameter = new object[] { /*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*IF:IsGenericParameterCallback*/fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.@ReturnTypeName, /*IF:IsGenericParameterCallback*//*NOTE*/(Func<fastCSharp.net.returnValue<object>, bool>)(object)/*NOTE*//*NOT:IsGenericParameterCallback*/(Func<fastCSharp.net.returnValue<@GenericReturnType.FullName>, bool>)/*NOT:IsGenericParameterCallback*/fastCSharp.net.tcp.commandServer.socket.callbackHttp/*IF:IsOutputParameter*//*IF:IsOutputParameter*//*IF:IsVerifyMethod*//*AT:IsVerifyMethod*//*IF:IsVerifyMethod*/<@OutputParameterTypeName, @GenericReturnType.FullName>.Get(httpPage, ref ouputParameter)/*IF:IsGenericParameterCallback*/)/*IF:IsGenericParameterCallback*/ };
                            #endregion IF MethodIsReturn
                            #region NOT MethodIsReturn
                            invokeParameter = new object[] { /*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*IF:IsGenericParameterCallback*/fastCSharp.code.cSharp.tcpBase.GetGenericParameterCallback(ref inputParameter.@ReturnTypeName, /*IF:IsGenericParameterCallback*//*NOTE*/(Func<fastCSharp.net.returnValue<object>, bool>)(object)/*NOTE*//*NOT:IsGenericParameterCallback*/(Func<fastCSharp.net.returnValue, bool>)/*NOT:IsGenericParameterCallback*/fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage)/*IF:IsGenericParameterCallback*/)/*IF:IsGenericParameterCallback*/ };
                            #endregion NOT MethodIsReturn
                            #endregion IF IsInvokeGenericMethod
                            #region IF MethodType.Type.IsGenericType
                            fastCSharp.code.cSharp.tcpCall.InvokeGenericTypeMethod(ref inputParameter.@TypeGenericParameterName, "@MethodIndexName"/*IF:Method.Method.IsGenericMethod*/, inputParameter.@GenericParameterTypeName/*IF:Method.Method.IsGenericMethod*/, invokeParameter);
                            #endregion IF MethodType.Type.IsGenericType
                            #region NOT MethodType.Type.IsGenericType
                            #region IF Method.Method.IsGenericMethod
                            fastCSharp.code.cSharp.tcpCall.InvokeGenericMethod(@MethodType.FullName/**/.@GenericTypeServerName/**/.@GenericMethodInfoName, inputParameter.@GenericParameterTypeName, invokeParameter);
                            #endregion IF Method.Method.IsGenericMethod
                            #region NOT Method.Method.IsGenericMethod
                            #region IF MethodIsReturn
                            @OutputParameterTypeName outputParameter = new @OutputParameterTypeName();
                            @MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*AT:ParameterRef*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue<MethodReturnType.FullName>, bool>)/*NOTE*/fastCSharp.net.tcp.commandServer.socket.callbackHttp/*IF:IsOutputParameter*//*IF:IsOutputParameter*//*IF:IsVerifyMethod*//*AT:IsVerifyMethod*//*IF:IsVerifyMethod*/<@OutputParameterTypeName, @GenericReturnType.FullName>.Get(httpPage, ref outputParameter));
                            #endregion IF MethodIsReturn
                            #region NOT MethodIsReturn
                            @MethodType.FullName/**/.@GenericTypeServerName/**/.@MethodIndexGenericName(/*IF:ClientParameterName*/commandSocket, /*IF:ClientParameterName*//*LOOP:MethodParameters*//*AT:ParameterRef*/inputParameter.@ParameterName, /*LOOP:MethodParameters*//*NOTE*/(Func<fastCSharp.net.returnValue, bool>)/*NOTE*/fastCSharp.net.tcp.commandServer.socket.callbackHttp.Get(httpPage));
                            #endregion NOT MethodIsReturn
                            #endregion NOT Method.Method.IsGenericMethod
                            #endregion NOT MethodType.Type.IsGenericType
                            #endregion IF IsAsynchronousCallback
                            #region NOT IsAsynchronousCallback
                            httpPage.Response(new @MethodMergeName { Socket = commandSocket/*IF:IsInputParameter*/, InputParameter = inputParameter/*IF:IsInputParameter*/ }.Get());
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
            #endregion NOT IsNullMethod
            #endregion LOOP MethodIndexs
        }
        #endregion PART SERVER
        #region PART CLIENT
        /// <summary>
        /// TCP调用客户端
        /// </summary>
        public/*NOTE*/ partial/*NOTE*/ class @ServiceName
        {
            #region IF ServiceAttribute.IsSegmentation
            #region LOOP MethodIndexs
            #region NOT IsNullMethod
            #region FROMNAME Parameter
            #endregion FROMNAME Parameter
            #endregion NOT IsNullMethod
            #endregion LOOP MethodIndexs
            #endregion IF ServiceAttribute.IsSegmentation
            /// <summary>
            /// 默认TCP调用服务器端配置信息
            /// </summary>
            protected internal static readonly fastCSharp.code.cSharp.tcpServer defaultTcpServer;
            /// <summary>
            /// 默认客户端TCP调用
            /// </summary>
            public static readonly fastCSharp.net.tcp.commandClient Default;
            static @ServiceName()
            {
                defaultTcpServer = fastCSharp.code.cSharp.tcpServer.GetTcpCallConfig("@ServiceName"/*IF:TcpServerAttributeType*/, typeof(@TcpServerAttributeType)/*IF:TcpServerAttributeType*/);
                #region IF ServiceAttribute.IsSegmentation
                defaultTcpServer.IsServer = false;
                #endregion IF ServiceAttribute.IsSegmentation
                #region NOT ServiceAttribute.IsSegmentation
                if (defaultTcpServer.IsServer) fastCSharp.log.Error.Add("请确认 @ServiceName 服务器端是否本地调用", null, false);
                #endregion NOT ServiceAttribute.IsSegmentation
                Default = new fastCSharp.net.tcp.commandClient(defaultTcpServer, @MaxCommandLength/*IF:IsVerifyMethod*/, new @TcpVerifyMethodType()/*IF:IsVerifyMethod*//*NOT:IsVerifyMethod*//*IF:ServiceAttribute.VerifyType*/, new @TcpVerifyType()/*IF:ServiceAttribute.VerifyType*//*NOT:IsVerifyMethod*/);
                if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(typeof(@ServiceName));
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
        #endregion PART CLIENT
        #region PART REMEMBER
        #region IF ServiceAttribute.IsIdentityCommand
        #region IF ServiceAttribute.IsRememberIdentityCommand
        /// <summary>
        /// TCP服务
        /// </summary>
        public partial class @ServiceName
        {
            /// <summary>
            /// 命令序号记忆数据
            /// </summary>
            private static fastCSharp.keyValue<string, int>[] @RememberIdentityCommeandName()
            {
                fastCSharp.keyValue<string, int>[] names = new fastCSharp.keyValue<string, int>[@MethodIndexs.Length];
                #region LOOP MethodIndexs
                #region NOT IsNullMethod
                names[@MethodIndex].Set(@"@Method.MethodKeyFullName", @MethodIndex);
                #endregion NOT IsNullMethod
                #endregion LOOP MethodIndexs
                return names;
            }
        }
        #endregion IF ServiceAttribute.IsRememberIdentityCommand
        #endregion IF ServiceAttribute.IsIdentityCommand
        #endregion PART REMEMBER
        #endregion IF IsAllType
        #endregion PART CLASS
    }
    #region NOTE
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 默认命名空间
        /// </summary>
        public partial class DefaultNamespace
        {
            /// <summary>
            /// 调用参数代码
            /// </summary>
            public class ParameterPart : fastCSharp.code.cSharp.template.TcpCall { }
            /// <summary>
            /// 客服端代码
            /// </summary>
            public class ClientPart : fastCSharp.code.cSharp.template.TcpCall { }
        }
        /// <summary>
        /// 获取该函数的类型
        /// </summary>
        public class MethodType
        {
            /// <summary>
            /// 类型全名
            /// </summary>
            public partial class FullName
            {
                /// <summary>
                /// TCP函数调用
                /// </summary>
                /// <param name="value">调用参数</param>
                /// <returns>返回值</returns>
                public object MethodGenericName(params object[] value)
                {
                    return null;
                }
                /// <summary>
                /// TCP调用
                /// </summary>
                public class GenericTypeServerName : pub.FullName.GenericTypeServerName
                {
                    /// <summary>
                    /// 字段/属性调用
                    /// </summary>
                    public static object PropertyName = null;
                }
            }
        }
        /// <summary>
        /// 返回值类型
        /// </summary>
        public class ReturnType : pub { }
        /// <summary>
        /// 返回值类型
        /// </summary>
        public class MethodReturnType : pub { }
        /// <summary>
        /// 参数类型
        /// </summary>
        public class ParameterType : pub { }
        /// <summary>
        /// 带引用修饰的参数名称
        /// </summary>
        public class ParameterTypeRefName : pub { }
        /// <summary>
        /// 函数泛型参数类型
        /// </summary>
        public class GenericParameterType : pub { }
        /// <summary>
        /// 函数泛型返回值类型
        /// </summary>
        public class GenericReturnType : pub { }
        /// <summary>
        /// TCP调用
        /// </summary>
        public class GenericTypeServerName
        {
            /// <summary>
            /// TCP函数调用
            /// </summary>
            /// <param name="value">调用参数</param>
            /// <returns>返回值</returns>
            public static object MethodIndexGenericName(params object[] value)
            {
                return null;
            }
            /// <summary>
            /// TCP函数调用
            /// </summary>
            /// <param name="value">调用参数</param>
            /// <returns>返回值</returns>
            public static object MethodGenericName(params object[] value)
            {
                return null;
            }
            /// <summary>
            /// 设置TCP服务调用配置
            /// </summary>
            /// <param name="value">TCP服务调用配置</param>
            public static void _setTcpServerAttribute_(params object[] value)
            {
            }
            /// <summary>
            /// 泛型函数信息
            /// </summary>
            public static readonly System.Reflection.MethodInfo GenericMethodInfoName = null;
        }
        /// <summary>
        /// TCP配置类型
        /// </summary>
        public class TcpServerAttributeType { }
        /// <summary>
        /// TCP验证类型
        /// </summary>
        public class TcpVerifyType : fastCSharp.code.cSharp.tcpBase.ITcpVerify
        {
            /// <summary>
            /// TCP客户端同步验证
            /// </summary>
            /// <param name="socket">同步套接字</param>
            /// <returns>是否通过验证</returns>
            public bool Verify(fastCSharp.net.tcp.commandServer.socket socket) { return false; }
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <param name="socket">TCP调用客户端</param>
            /// <returns>是否通过验证</returns>
            public bool Verify(fastCSharp.net.tcp.commandClient.socket client) { return false; }
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <returns>是否通过验证</returns>
            public bool Verify() { return false; }
        }
        /// <summary>
        /// 客户端验证方法类型
        /// </summary>
        public class TcpVerifyMethodType : fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod, fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<fastCSharp.net.tcp.commandClient>
            , fastCSharp.code.cSharp.tcpBase.ITcpClientVerifyMethod<fastCSharp.code.cSharp.template.tcpServer.TypeNameDefinition.tcpClient>
        {
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <param name="socket">TCP调用客户端</param>
            /// <returns>是否通过验证</returns>
            public bool Verify(fastCSharp.net.tcp.commandClient client) { return false; }
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <returns>是否通过验证</returns>
            public bool Verify() { return false; }
            /// <summary>
            /// TCP客户端验证
            /// </summary>
            /// <param name="socket">TCP调用客户端</param>
            /// <returns>是否通过验证</returns>
            public bool Verify(fastCSharp.code.cSharp.template.tcpServer.TypeNameDefinition.tcpClient client) { return false; }
        }
        /// <summary>
        /// 客户端验证方法类型
        /// </summary>
        public class TcpTimeVerifyMethodType : fastCSharp.net.tcp.timeVerifyServer.tcpCall<TcpTimeVerifyMethodType>
        {
        }
    }
    #endregion NOTE
}