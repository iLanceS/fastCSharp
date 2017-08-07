using System;
#pragma warning disable 649

namespace fastCSharp.code.cSharp.template
{
    class webCall : pub
    {
        #region PART CLASS
        #region IF Methods.Length
        /// <summary>
        /// WEB服务器
        /// </summary>
        public partial class webServer : fastCSharp.net.tcp.http.domainServer.viewServer<@SessionType.FullName>
        {
            #region NOTE
            const int MaxPostDataSize = 0;
            const int MaxMemoryStreamSize = 0;
            const int MethodIndex = 0;
            pub.FullName[] Methods = null;
            const bool IsOnlyPost = false;
            const bool IsPool = false;
            #endregion NOTE
            /// <summary>
            /// WEB调用处理索引集合
            /// </summary>
            protected override string[] calls
            {
                get
                {
                    string[] names = new string[@Methods.Length];
                    #region LOOP Methods
                    names[@MethodIndex] = "@CallName";
                    #endregion LOOP Methods
                    return names;
                }
            }
            /// <summary>
            /// WEB调用处理
            /// </summary>
            /// <param name="callIndex"></param>
            /// <param name="socket"></param>
            /// <param name="socketIdentity"></param>
            protected override void call(int callIndex, fastCSharp.net.tcp.http.socketBase socket, long socketIdentity)
            {
                switch (callIndex)
                {
                    #region LOOP Methods
                    case @MethodIndex:
                        #region IF IsAjaxLoad
                        loadAjax<@MethodCallName, @WebCallMethodType.FullName>(socket, socketIdentity, @MethodCallName/**/.Get(), /*IF:TypeAttribute.IsPool*/ fastCSharp.typePool<@WebCallMethodType.FullName>.Pop() ??/*IF:TypeAttribute.IsPool*/ new @WebCallMethodType.FullName());
                        #endregion IF IsAjaxLoad
                        #region NOT IsAjaxLoad
                        load<@MethodCallName, @WebCallMethodType.FullName>(socket, socketIdentity, @MethodCallName/**/.Get(), /*IF:TypeAttribute.IsPool*/ fastCSharp.typePool<@WebCallMethodType.FullName>.Pop() ??/*IF:TypeAttribute.IsPool*/ new @WebCallMethodType.FullName(), @MaxPostDataSize, @MaxMemoryStreamSize/*PUSH:Attribute*/, @IsOnlyPost/*PUSH:Attribute*//*PUSH:TypeAttribute*/, @IsPool/*PUSH:TypeAttribute*/);
                        #endregion NOT IsAjaxLoad
                        return;
                    #endregion LOOP Methods
                }
            }
            #region LOOP Methods
            #region IF IsParameter
            #region IF Attribute.IsSerializeBox
            [fastCSharp.emit.boxSerialize]
            #endregion IF Attribute.IsSerializeBox
            struct @ParameterTypeName
            {
                #region LOOP MethodParameters
                public @ParameterType.GenericParameterType.FullName @ParameterName;
                #endregion LOOP MethodParameters
            }
            #endregion IF IsParameter
            private sealed class @MethodCallName : fastCSharp.code.cSharp.webCall.callPool<@MethodCallName, @WebCallMethodType.FullName/*IF:IsParameter*/, @ParameterTypeName/*IF:IsParameter*/>
            {
                private @MethodCallName() : base() { }
                public override bool Call()
                {
                    try
                    {
                        #region IF IsParameter
                        #region IF Attribute.IsFirstParameter
                        if (WebCall.ParseParameterAny(ref Parameter/*PUSH:FristParameter*/.@ParameterName/*PUSH:FristParameter*/))
                            #endregion IF Attribute.IsFirstParameter
                            #region NOT Attribute.IsFirstParameter
                            if (WebCall.ParseParameter(ref Parameter))
                            #endregion NOT Attribute.IsFirstParameter
                            #endregion IF IsParameter
                            {
                                WebCall./*PUSH:Method*/@MethodGenericName/*PUSH:Method*/(/*LOOP:MethodParameters*//*AT:ParameterRef*/Parameter.@ParameterName/*AT:ParameterJoin*//*LOOP:MethodParameters*/);
                                return true;
                            }
                    }
                    finally
                    {
                        WebCall = null;
                        typePool<@MethodCallName>.PushNotNull(this);
                    }
                    #region IF IsParameter
                    return false;
                    #endregion IF IsParameter
                }
                [System.Runtime.CompilerServices.MethodImpl((System.Runtime.CompilerServices.MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
                public static @MethodCallName Get()
                {
                    @MethodCallName call = fastCSharp.typePool<@MethodCallName>.Pop();
                    if (call == null) call = new @MethodCallName();
                    #region IF IsParameter
                    else call.Parameter = new @ParameterTypeName();
                    #endregion IF IsParameter
                    return call;
                }
            }
            #endregion LOOP Methods
        }
        #endregion IF Methods.Length
        #endregion PART CLASS
    }
    /// <summary>
    /// CSharp模板公用模糊类型
    /// </summary>
    internal partial class pub
    {
        /// <summary>
        /// 类型全名
        /// </summary>
        public partial class FullName : fastCSharp.code.cSharp.webCall.IWebCall
        {
            /// <summary>
            /// HTTP请求头部设置
            /// </summary>
            public fastCSharp.net.tcp.http.requestHeader RequestHeader { set { } }
            /// <summary>
            /// HTTP请求表单设置
            /// </summary>
            public fastCSharp.net.tcp.http.requestForm RequestForm { set { } }
            /// <summary>
            /// 解析web调用参数
            /// </summary>
            /// <typeparam name="parameterType">web调用参数类型</typeparam>
            /// <param name="parameter">web调用参数</param>
            /// <returns>是否成功</returns>
            public bool ParseParameter<parameterType>(ref parameterType parameter)
                where parameterType : struct { return false; }
            /// <summary>
            /// 是否使用WEB页面回收池
            /// </summary>
            public bool IsPagePool { set { } }
        }
        /// <summary>
        /// 获取该函数的类型
        /// </summary>
        public class WebCallMethodType
        {
            /// <summary>
            /// 类型全名
            /// </summary>
            public partial class FullName : fastCSharp.code.cSharp.webCall.call<FullName>, fastCSharp.code.cSharp.webCall.IWebCall
            {
                /// <summary>
                /// web调用
                /// </summary>
                /// <param name="value">调用参数</param>
                /// <returns>返回值</returns>
                public object MethodGenericName(params object[] value)
                {
                    return null;
                }
            }
        }
    }
}
